using Managers;
using Michsky.UI.ModernUIPack;
using System.Diagnostics;
using UnityEngine;
using Utils;

namespace GameUI
{
    public class UI_Modal : UI_Popup
    {
        
        [Header("Manager")]
         [SerializeField] private NotificationManager _notification;
         [SerializeField] private UIManagerNotification _uiManagerNotification;


         [Header("Canvas Order")] [SerializeField]
         private int canvasOrder = 100;
         

         protected override void Init()
         {
             base.Init();
             
             //모달은 가장 상위에 뜨도록 order를 추가 설정
             
             Canvas canvas = GetComponent<Canvas>();

             canvas.sortingOrder = canvas.sortingOrder + canvasOrder;

         }

         //모달 ui를 실제로 show하려면 이 메서드 호출
        public override void Show()
        {
            base.Show();
            _notification.OpenNotification();
        }

        //모달 내용 변경
        public void SetContent(Define_LDH.NotifyType notifyType, string title, string description)
        {
            var notifyStyle = Manager.UI.notifyStyle.GetStyle(notifyType);
            
            //색깔 변경
            _uiManagerNotification.background.color = notifyStyle.backgroundColor;
            
            //아이콘 바꾸기
            _notification.icon = notifyStyle.icon;
            
            //title 바꾸기
            _notification.title = title;

            // description 바꾸기
            _notification.description = description;
            
        }
        
        
    }
}