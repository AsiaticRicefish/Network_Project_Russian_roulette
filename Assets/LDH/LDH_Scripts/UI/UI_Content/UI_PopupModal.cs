using Michsky.UI.ModernUIPack;
using UnityEngine;
using Utils;

namespace GameUI
{
    public class UI_PopupModal : UI_Popup
    {
        private ModalWindowManager _modalWindowManager;

        [SerializeField] private Sprite _errorIcon;
        [SerializeField] private Sprite _helpIcon;
        [SerializeField] private Sprite _notifyIcon;
        [SerializeField] private Sprite _checkIcon;
        
        
        
        protected override void Init()
        {
            base.Init();

            _modalWindowManager = GetComponentInChildren<ModalWindowManager>();
        }


        //모달 ui를 실제로 show하려면 이 메서드 호출
        public override void Show()
        {
            base.Show();
            _modalWindowManager.OpenWindow();
        }

        //모달 내용 변경
        public void SetContent(Define_LDH.NotifyType notifyType, string title, string description)
        {
            //아이콘 바꾸기
            _modalWindowManager.icon = notifyType switch
            {
                Define_LDH.NotifyType.Error => _errorIcon,
                Define_LDH.NotifyType.Check => _checkIcon,
                Define_LDH.NotifyType.Help => _helpIcon,
                Define_LDH.NotifyType.Notify => _notifyIcon,
                _ => _modalWindowManager.icon
            };
            
            //title 바꾸기
            _modalWindowManager.titleText = title;

            // description 바꾸기
            _modalWindowManager.descriptionText = description;
            
        }


    }
}