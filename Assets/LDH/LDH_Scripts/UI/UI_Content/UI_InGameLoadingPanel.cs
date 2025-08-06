using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_InGameLoadingPanel : MonoBehaviour
    {
        private Image _image;
        [SerializeField] private float fadeTime = 0.5f;
        
        private void Awake()
        {
            _image = GetComponent<Image>();
            _image.color = Color.black;
            PlayerManager.Instance.OnAddPlayer += FadeIn;
        }

        private void OnDestroy()
        {
            PlayerManager.Instance.OnAddPlayer-= FadeIn;
        }

        private void FadeIn()
        {
            if(PlayerManager.Instance.GetAllPlayers().Count!=2) return;
            Debug.Log("[UI_InGameLoadingPanel] FadeIn 호출됨");
            //Dotween으로 fadetime 동안 color alpha 밝게
            
            _image.DOFade(0f, fadeTime)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
                {
                    gameObject.SetActive(false); // 페이드 완료 후 비활성화
                    Debug.Log("[UI_InGameLoadingPanel] 페이드 완료 후 비활성화");
                });
        }
    }
}