using GameUI;
using Managers;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace GameUI
{
    public class UI_Setting : UI_Popup
    {
        [SerializeField] private GameObject settingPanelRoot;

        private RectTransform panelRect;
        private CanvasGroup panelCanvasGroup;

        protected override void Init()
        {
            base.Init();
            
            panelRect = settingPanelRoot.GetComponent<RectTransform>();

            panelCanvasGroup = Util_LDH.GetOrAddComponent<CanvasGroup>(settingPanelRoot);
        }

        private void OnEnable()
        {
           
        }
        
        

        private void ShowEffect()
        {
            AnimationManager.Instance
                .Create(panelRect)
                .FadeIn(panelCanvasGroup, 0.3f)
                .MoveTo(Vector2.zero, 0.3f) // 정위치로 올라오기
                .Play();
        }


        public override void Close()
        {
            AnimationManager.Instance
                .Create(panelRect)
                .FadeOut(panelCanvasGroup, 0.3f)
                .MoveBy(Vector2.down * 100f, 0.3f)
                .OnComplete(() => base.Close()) // 비활성화 처리
                .Play();
        }
        
    }
}