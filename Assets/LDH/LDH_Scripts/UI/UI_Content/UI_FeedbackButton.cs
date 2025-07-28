using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using DG.Tweening;
using UnityEngine.Events;

namespace GameUI
{
    /// <summary>
    /// 성공/실패 여부에 따라 Fill 애니메이션과 아이콘, 색상 피드백을 보여주는 버튼 UI
    /// 외부에서 비동기적으로 결과(bool)를 전달받아 처리
    /// </summary>
    public class UI_FeedbackButton : MonoBehaviour
    {
        [Header("UI Events")]
        public UnityEvent onSuccessEvent;
        public UnityEvent onFailEvent;
        
        [Header("UI Components")]
        [SerializeField] private Image _fillImage;
        [SerializeField] private Image _iconImage;

        [Header("Color")]
        [SerializeField] private Color _defaultColor;
        
        
        [Header("Config")]
        [SerializeField] private float _fillDuration = 1.0f;
        [SerializeField] private float _feedbackDuration = 0.3f;
        
        
        private bool _isChecking;
        
        
        public void Awake() => Clear();
        public void Start() => Subscribe();


        public void Subscribe()
        {
            // 클릭 시 시각 효과 + 테스트 Firebase 시뮬레이션 실행
            UI_Base.BindUIEvent(gameObject, (_) => StartChecking());
            
        }
        
        
        /// <summary>
        /// 버튼 클릭 시 호출됨. fill 애니메이션과 함께 결과 대기 시작
        /// </summary>
        public void StartChecking()
        {
            
            if (_isChecking) return;
            _isChecking = true;
            
            _fillImage.fillAmount = 0f;
            _fillImage.color = _defaultColor;
            _fillImage.enabled = true;
            _iconImage.enabled = false;
            
            
            _fillImage.DOFillAmount(1f, _fillDuration)
                .SetEase(Ease.Linear);
            
            
        }
        
        /// <summary>
        /// 외부에서 결과를 전달받으면 호출. 시각적 피드백 및 콜백 처리
        /// </summary>
        /// <param name="isSuccess">성공 여부</param>
        public void ShowResult(bool isSuccess)
        {
            if (!_isChecking) return;
            _isChecking = false;

            PlayResultFeedback(isSuccess);

            if (isSuccess) onSuccessEvent?.Invoke();
            else onFailEvent?.Invoke();
        }
        
        /// <summary>
        /// 성공/실패 결과에 따라 색상, 아이콘, 애니메이션 효과를 보여줌
        /// </summary>
        private void PlayResultFeedback(bool isSuccess)
        {
            var notifyStyle =
                Manager.UI.notifyStyle.GetStyle(isSuccess ? Define_LDH.NotifyType.Check : Define_LDH.NotifyType.Error);
            
            _fillImage.DOColor(notifyStyle.backgroundColor, _feedbackDuration);

            _iconImage.sprite = notifyStyle.icon;
            _iconImage.enabled = true;
            
            //todo: 애니메이션 매니저 이용하기
            _iconImage.DOFade(1f, _feedbackDuration)
                .SetEase(Ease.OutQuad)
                .SetDelay(0.05f);
        }
        
        
        /// <summary>
        /// 시각 요소 초기화 (비활성화 상태로 리셋)
        /// </summary>
        public void Clear()
        {
            _fillImage.enabled = false;
            _fillImage.fillAmount = 0f; 
            
            _iconImage.sprite = Manager.UI.notifyStyle.GetStyle(Define_LDH.NotifyType.Check).icon;

            _isChecking = false;
        }

        
    }
}