using DG.Tweening;
using System;
using UnityEngine;

namespace LDH_Animation
{
    /// <summary>
    /// DOTween 기반의 애니메이션 빌더 클래스.
    /// 순차적/동시적 애니메이션을 간편하게 구성할 수 있음.
    /// 
    /// 사용 예시:
    /// new AnimationBuilder(transform)
    ///     .MoveTo(pos1, 1f)
    ///     .JoinNext().FadeIn(canvasGroup)
    ///     .Delay(0.5f)
    ///     .MoveTo(pos2, 1f)
    ///     .OnComplete(() => Debug.Log("Done"))
    ///     .Play();
    /// </summary>

    public class AnimationBuilder
    {
        private Sequence _sequence;                     // DOTween 애니메이션 시퀀스
        private Transform _transformTarget;             // 일반 Transform 대상
        private RectTransform _rectTransformTarget;     // UI RectTransform 대상 (UI용)

        private Ease _defaultEase = Ease.Unset;         // 기본 Ease 설정
        private bool _isJoinMode = false;               // 다음 Tween을 Join(동시재생)으로 처리할지 여부

        /// <summary>
        /// AnimationBuilder를 생성. Transform 또는 RectTransform을 인자로 받음
        /// </summary>
        public AnimationBuilder(Transform transformTarget)
        {
            _sequence = DOTween.Sequence();
            _transformTarget = transformTarget;

            // UI인 경우 따로 캐싱
            if (transformTarget is RectTransform rectTransform)
                _rectTransformTarget = rectTransform;
        }

        /// <summary>
        /// 이후 추가되는 Tween에 기본 Ease 값을 적용
        /// </summary>
        public AnimationBuilder SetEase(Ease ease)
        {
            _defaultEase = ease;
            return this;
        }

        /// <summary>
        /// 지연 시간 추가 (AppendInterval 사용)
        /// </summary>
        public AnimationBuilder Delay(float seconds)
        {
            _sequence.AppendInterval(seconds);
            return this;
        }

        /// <summary>
        /// 애니메이션 완료 시 실행할 콜백 등록
        /// </summary>
        public AnimationBuilder OnComplete(Action onComplete)
        {
            _sequence.OnComplete(() => onComplete?.Invoke());
            return this;
        }

        /// <summary>
        /// 다음 Tween을 Join(병렬 실행)으로 처리하도록 설정
        /// </summary>
        public AnimationBuilder JoinNext()
        {
            _isJoinMode = true;
            return this;
        }

        /// <summary>
        /// 내부 Tween 실행 방식 결정 (Append or Join)
        /// </summary>
        private T TweenWrapper<T>(T tween) where T : Tween
        {
            if (_isJoinMode)
            {
                _sequence.Join(tween);     // 동시 실행
                _isJoinMode = false;       // 한 번만 join 적용
            }
            else
            {
                _sequence.Append(tween);   // 순차 실행
            }

            return tween;
        }

        /// <summary>
        /// CanvasGroup을 1로 페이드 인 (UI 전용)
        /// </summary>
        public AnimationBuilder FadeIn(CanvasGroup canvasGroup, float duration = 0.5f)
        {
            TweenWrapper(canvasGroup.DOFade(1f, duration).SetEase(_defaultEase));
            return this;
        }

        /// <summary>
        /// CanvasGroup을 0으로 페이드 아웃 (UI 전용)
        /// </summary>
        public AnimationBuilder FadeOut(CanvasGroup canvasGroup, float duration = 0.5f)
        {
            TweenWrapper(canvasGroup.DOFade(0f, duration).SetEase(_defaultEase));
            return this;
        }

        /// <summary>
        /// 대상 오브젝트에 Shake 애니메이션 적용
        /// - RectTransform: anchorPosition 기준
        /// - Transform: worldPosition 기준
        /// </summary>
        public AnimationBuilder Shake(float duration = 0.3f, float strength = 20f, int vibrato = 10)
        {
            if (_rectTransformTarget != null)
            {
                TweenWrapper(_rectTransformTarget
                    .DOShakeAnchorPos(duration, strength, vibrato)
                    .SetEase(_defaultEase)
                    .SetUpdate(true)); // TimeScale 무시
            }
            else if (_transformTarget != null)
            {
                TweenWrapper(_transformTarget
                    .DOShakePosition(duration, strength, vibrato)
                    .SetEase(_defaultEase));
            }

            return this;
        }

        /// <summary>
        /// 현재 위치에서 지정 offset만큼 이동
        /// </summary>
        public AnimationBuilder MoveBy(Vector3 offset, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
            {
                Vector2 start = _rectTransformTarget.anchoredPosition;
                TweenWrapper(_rectTransformTarget
                    .DOAnchorPos(start + (Vector2)offset, duration)
                    .SetEase(_defaultEase));
            }
            else if (_transformTarget != null)
            {
                TweenWrapper(_transformTarget
                    .DOMove(_transformTarget.position + offset, duration)
                    .SetEase(_defaultEase));
            }

            return this;
        }

        /// <summary>
        /// 지정 위치로 이동
        /// </summary>
        public AnimationBuilder MoveTo(Vector3 targetPos, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
            {
                TweenWrapper(_rectTransformTarget
                    .DOAnchorPos((Vector2)targetPos, duration)
                    .SetEase(_defaultEase));
            }
            else if (_transformTarget != null)
            {
                TweenWrapper(_transformTarget
                    .DOMove(targetPos, duration)
                    .SetEase(_defaultEase));
            }

            return this;
        }

        /// <summary>
        /// from → to 위치로 애니메이션 (초기 위치 설정 포함)
        /// </summary>
        public AnimationBuilder MoveToFrom(Vector3 fromPos, Vector3 toPos, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
            {
                _rectTransformTarget.anchoredPosition = fromPos;
                TweenWrapper(_rectTransformTarget
                    .DOAnchorPos((Vector2)toPos, duration)
                    .SetEase(_defaultEase));
            }
            else if (_transformTarget != null)
            {
                _transformTarget.position = fromPos;
                TweenWrapper(_transformTarget
                    .DOMove(toPos, duration)
                    .SetEase(_defaultEase));
            }

            return this;
        }

        /// <summary>
        /// 구성한 애니메이션 시퀀스 실행
        /// </summary>
        public void Play()
        {
            _sequence.Play();
        }
    }
}