using System;
using UnityEngine;
using UnityEngine.EventSystems;
using Utils;

namespace GameUI
{
    public abstract class UI_Base : MonoBehaviour
    {

        protected void Awake() => Init();
        protected abstract void Init();


        #region Event Binding

        /// <summary>
        /// 지정된 GameObject에 UI 이벤트(Click, Enter, Exit, Drag)를 바인딩하는 정적 메서드.
        /// UI_EventHandler 컴포넌트를 자동으로 추가하며, 중복 바인딩을 방지하기 위해 먼저 이벤트를 제거한 후 등록.
        /// </summary>
        /// <param name="uiGameObject">이벤트를 바인딩할 대상 GameObject</param>
        /// <param name="eventAction">실행할 이벤트 핸들러</param>
        /// <param name="eventType">바인딩할 이벤트 종류(Default : Click)</param>
        public static void BindUIEvent(GameObject uiGameObject, Action<PointerEventData> eventAction,
            Define_LDH.UIEvent eventType = Define_LDH.UIEvent.Click)
        {
            var eventHandler = Util_LDH.GetOrAddComponent<UI_EventHandler>(uiGameObject);

            switch (eventType)
            {
                case Define_LDH.UIEvent.Click:
                    eventHandler.OnClickHandler -= eventAction;
                    eventHandler.OnClickHandler += eventAction;
                    break;
                case Define_LDH.UIEvent.PointEnter:
                    eventHandler.OnEnterHandler -= eventAction;
                    eventHandler.OnEnterHandler += eventAction;
                    break;
                case Define_LDH.UIEvent.PointExit:
                    eventHandler.OnExitHandler -= eventAction;
                    eventHandler.OnExitHandler += eventAction;
                    break;
                case Define_LDH.UIEvent.Drag:
                    eventHandler.OnDragHandler -= eventAction;
                    eventHandler.OnDragHandler += eventAction;
                    break;
            }
        }

        #endregion


        #region UI Show/Hide

        

        #endregion
  
    }
}