using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

namespace GameUI
{
    /// <summary>
    /// UI 오브젝트에서의 이벤트(PointerClick, Drag, Enter, Exit) 를 바인딩할 수 있는 핸들러 클래스.
    /// 외부에서 Action을 등록하여 동적으로 이벤트를 처리할 수 있다.
    /// </summary>
    public class UI_EventHandler : MonoBehaviour, IPointerClickHandler, IDragHandler, IPointerEnterHandler, IPointerExitHandler
    {
        /// <summary>
        /// 클릭 시 실행될 이벤트
        /// </summary>
        public Action<PointerEventData> OnClickHandler;
        /// <summary>
        /// 마우스 커서가 오브젝트에 들어갈 때 실행될 이벤트
        /// </summary>
        public Action<PointerEventData> OnEnterHandler;
        /// <summary>
        /// 마우스 커서가 오브젝트에서 나갈 때 실행될 이벤트
        /// </summary>
        public Action<PointerEventData> OnExitHandler;
        /// <summary>
        /// 드래그 중 실행될 이벤트
        /// </summary>
        public Action<PointerEventData> OnDragHandler;
        
        public void OnPointerClick(PointerEventData eventData)
        {
            OnClickHandler?.Invoke(eventData);
        }
        

        public void OnPointerEnter(PointerEventData eventData)
        {
            OnEnterHandler?.Invoke(eventData);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            OnExitHandler?.Invoke(eventData);
        }

        public void OnDrag(PointerEventData eventData)
        {
            OnDragHandler?.Invoke(eventData);
        }
    }
}