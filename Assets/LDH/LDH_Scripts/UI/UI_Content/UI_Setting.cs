using GameUI;
using Managers;
using Michsky.UI.ModernUIPack;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using Utils;
using UIManager = Managers.UIManager;

namespace GameUI
{
    public class UI_Setting : UI_Popup
    {
        public ModalWindowManager settingModal;
        [SerializeField] private UI_VolumeSetting _volumeSetting;

        public bool isInitialized = false;
        
        protected override void Init()
        {
            base.Init();
        }
        

        private void OnEnable()
        {
           Show();
        }
        

        public override void Show()
        {
           settingModal.OpenWindow();
        }

        public override void Close()
        {
            StartCoroutine(CloseWithDelay());
        }

        private IEnumerator CloseWithDelay()
        {
            settingModal.CloseWindow();
            
            if(isInitialized)
                yield return new WaitForSeconds(0.5f);
            
            Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_Setting);
        }
    }
}