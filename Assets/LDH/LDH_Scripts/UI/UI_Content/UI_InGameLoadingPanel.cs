using DG.Tweening;
using Managers;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_InGameLoadingPanel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _loadingText;
        private Image _image;
        private CanvasGroup _canvasGroup;

        [SerializeField] private float fadeTime = 0.5f;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _canvasGroup = GetComponent<CanvasGroup>();
            
            if (_canvasGroup == null)
            {
                _canvasGroup = gameObject.AddComponent<CanvasGroup>();
            }
            
            _canvasGroup.alpha = 1f; // 시작은 검정 화면
            
            
            PlayerManager.Instance.OnAddPlayer += FadeOutAfterLoading;
        }

        private void OnDestroy()
        {
            //PlayerManager.Instance.OnAddPlayer-= FadeOutAfterLoading;
        }

        private void FadeOutAfterLoading()
        {
            if(PlayerManager.Instance.GetAllPlayers().Count!=2) return;
            Debug.Log("[UI_InGameLoadingPanel] FadeIn 호출됨");
            //Dotween으로 fadetime 동안 color alpha 밝게
            
            Manager.Anim.Create(transform)
                .FadeOut(_canvasGroup, fadeTime)
                .OnComplete(() =>
                {
                    _loadingText.gameObject.SetActive(false);   
                    Debug.Log("[UI_InGameLoadingPanel] 페이드 완료, loading 텍스트 비활성화");

                })
                .Play();
        }
    }
}