using GameUI;
using System;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEngine;
using Utils;

namespace Managers
{
    public class UIManager : DesignPattern.Singleton<UIManager>
    {
        //stack 관리
        private int _order = 10;
        private Stack<UI_Popup> _popupStack = new();
        
        //UI Root 
        public static GameObject UIRoot { get; private set; }
        
        
        //전역 UI 관리
        //전역으로 사용되는 UI
        private Dictionary<Type, UI_Base> _globalUIDict = new();
        
        
        //Popup prefab path Folder Path
        [SerializeField] private string _popupPrefabFolder = "Prefabs/UI/Popup";
        

        private void Awake() => Init();

        private void Init()
        {
            SingletonInit();        //싱글톤 초기화
            
            InitUIRoot();           //UI Root를 생성
            
            Clear();                //Popup Stack 초기화
            
            InitGlobalUI(); // 전역 UI 생성 및 관리
            
        }

        private void InitUIRoot()
        {
            if (UIRoot == null)
            {
                UIRoot = new GameObject("@UIRoot");
                DontDestroyOnLoad(UIRoot);
            }
        }

        private void InitGlobalUI()
        {
            //todo: 전역 UI 생성, 딕셔너리 추가, enum으로 처리할지 고려중
            
        }


        #region Canvas Setting

        public void SetCanvas(GameObject go, bool sort = true)
        {
            Canvas canvas = Util_LDH.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            canvas.sortingOrder = sort ? _order++ : 0;
        }

        #endregion

        #region Global UI Control

        public T GetGlobalUI<T>(string name = null) where T : UI_Base
        {
            return _globalUIDict.TryGetValue(typeof(T), out var ui) ? ui as T : null;
        }

        public void ToggleGlobalUI<T>(bool isActive) where T : UI_Base
        {
            if (_globalUIDict.TryGetValue(typeof(T), out var ui))
            {
                ui.gameObject.SetActive(isActive);
            }
        }
        

        #endregion
        
        #region World UI Control
        ///todo : 텍스트 같은 world space UI
        // public T MakeWorldSapceUI<T>()
        // {
        //        
        // }
       
        #endregion
        
        

        #region Popup UI control
        
        public T ShowPopupUI<T>(string name = null) where T : UI_Popup
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(T).Name;
            
            T popup = Util_LDH.Instantiate<T>(Path.Combine(_popupPrefabFolder, name), UIRoot.transform);
            _popupStack.Push(popup);
            
            return popup;
        }

        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0)
                return;

            if (_popupStack.Peek() != popup)
            {
                Debug.Log($"[{GetType().Name}] pop up이 가장 위에 있는 팝업이 아닙니다..?");
                return;
            }

            ClosePopupUI();
        }
        
        
        public void ClosePopupUI()
        {
            if (_popupStack.Count == 0)
                return;

            UI_Popup popup = _popupStack.Pop();
            Destroy(popup.gameObject);
            popup = null;
            _order--;
        }
        
        public void CloseAllPopupUI()
        {
            while(_popupStack.Count > 0)
                ClosePopupUI();
        }

        #endregion
        
        

        public void Clear()
        {
            CloseAllPopupUI();
        }
        
    }
}