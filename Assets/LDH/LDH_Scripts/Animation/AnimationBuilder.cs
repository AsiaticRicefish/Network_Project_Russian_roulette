using DG.Tweening;
using System;
using UnityEngine;

namespace LDH_Animation
{
    public class AnimationBuilder
    {
        private Sequence _sequence;
        private Transform _transformTarget;
        private RectTransform _rectTransformTarget;

        private Ease _defaultEase = Ease.Unset;

        
        public AnimationBuilder(Transform transformTarget)
        {
            _sequence = DOTween.Sequence();
            
            //UI인 경우
            if (transformTarget is RectTransform rectTransform)
                _rectTransformTarget = rectTransform;
            
            _transformTarget = transformTarget;
        }
        
        public AnimationBuilder SetEase(Ease ease)
        {
            _defaultEase = ease;
            return this;
        }
        
        public AnimationBuilder Delay(float seconds)
        {
            _sequence.AppendInterval(seconds);
            return this;
        }

        public AnimationBuilder OnComplete(Action onComplete)
        {
            _sequence.OnComplete(() => onComplete?.Invoke());
            return this;
        }
        
        
        // FadeIn (UI 전용)
        public AnimationBuilder FadeIn(CanvasGroup canvasGroup, float duration = 0.5f)
        {
            _sequence.Append(canvasGroup.DOFade(1f, duration));
            return this;
        }

        // FadeOut (UI 전용)
        public AnimationBuilder FadeOut(CanvasGroup canvasGroup, float duration = 0.5f)
        {
            _sequence.Append(canvasGroup.DOFade(0f, duration));
            return this;
        }
        
        
        public AnimationBuilder Shake(float duration = 0.3f, float strength = 20f, int vibrato = 10)
        {
            if (_rectTransformTarget != null)
                _sequence.Join(_rectTransformTarget.DOShakeAnchorPos(duration, strength, vibrato).SetUpdate(true));
            else if (_transformTarget != null)
                _sequence.Join(_transformTarget.DOShakePosition(duration, strength, vibrato));

            return this;
        }

        public AnimationBuilder MoveBy(Vector3 offset, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
            {
                var start = _rectTransformTarget.anchoredPosition;
                _sequence.Append(_rectTransformTarget.DOAnchorPos(start + (Vector2)offset, duration).SetEase(Ease.OutCubic));
            }
            else if (_transformTarget != null)
            {
                _sequence.Append(_transformTarget.DOMove(_transformTarget.position + offset, duration).SetEase(Ease.OutCubic));
            }

            return this;
        }

        public AnimationBuilder MoveTo(Vector3 targetPos, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
                _sequence.Append(_rectTransformTarget.DOAnchorPos((Vector2)targetPos, duration).SetEase(Ease.OutCubic));
            else if (_transformTarget != null)
                _sequence.Append(_transformTarget.DOMove(targetPos, duration).SetEase(Ease.OutCubic));

            return this;
        }
        
        public AnimationBuilder MoveToFrom(Vector3 fromPos, Vector3 toPos, float duration = 0.5f)
        {
            if (_rectTransformTarget != null)
            {
                _rectTransformTarget.anchoredPosition = fromPos;
                _sequence.Append(_rectTransformTarget.DOAnchorPos((Vector2)toPos, duration).SetEase(Ease.OutCubic));
            }
            else if (_transformTarget != null)
            {
                _transformTarget.position = fromPos;
                _sequence.Append(_transformTarget.DOMove(toPos, duration).SetEase(Ease.OutCubic));
            }

            return this;
        }
        

        
        public void Play()
        {
            _sequence.Play();
        }
    }
}