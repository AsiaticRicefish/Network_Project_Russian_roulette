using Managers;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameUI
{
    public class UI_Shortcut : UI_Base
    {
        [SerializeField] private Button escButton;
        [SerializeField] private Image escButtonImage;
        
        private UI_Setting settingUI;
        private bool activate = false;
       
        protected override void Init()
        {
            activate = false;
            escButton.onClick.AddListener(OnEscButtonClicked);
        }

        public void InitSetting()
        {
            if (activate) return;

            settingUI ??= Manager.UI.GetGlobalUI(Define_LDH.GlobalUI.UI_Setting) as UI_Setting;
            settingUI.settingModal.onCancel.AddListener(OnEscButtonClicked);
            settingUI.isInitialized = true;
            
            activate = true;
        }

        private void Update()
        {
            if (activate && Input.GetKeyDown(KeyCode.Escape))
            {
                OnEscButtonClicked();
            }
        }

        /// <summary>
        /// ESC 버튼 눌렀을 때 호출: 코루틴 시작 래핑
        /// </summary>
        private void OnEscButtonClicked()
        {
            StartCoroutine(ToggleSettingUI());
        }

        /// <summary>
        /// 설정창 열고 닫는 토글 처리
        /// </summary>
        private IEnumerator ToggleSettingUI()
        {
            escButtonImage.raycastTarget = false;
            activate = false;

            if (!settingUI.gameObject.activeSelf)
            {
                Manager.UI.ShowGlobalUI(Define_LDH.GlobalUI.UI_Setting);
            }
            else
            {
                settingUI.Close();
                
            }
            
            yield return new WaitForSeconds(0.5f);
            escButtonImage.raycastTarget = true;
            activate = true;
        }
    }
}