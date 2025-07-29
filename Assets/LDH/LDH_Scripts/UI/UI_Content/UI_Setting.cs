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
        
        protected override void Init()
        {
            base.Init();
            //InitSubscribe();
        }
        

        private void OnEnable()
        {
           Show();
        }

        public void InitSubscribe()
        {
            settingModal.onCancel.AddListener(Close);
        }
        

        public override void Show()
        {
           settingModal.OpenWindow();
        }

        public override void Close()
        {
            settingModal.CloseWindow();
            Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_Setting);
        }
    }
}