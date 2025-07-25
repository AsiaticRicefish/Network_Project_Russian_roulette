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
        [SerializeField] private string _globalUIPrefabFolder = "Prefabs/UI/Global";

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
            string[] globalUIs = System.Enum.GetNames(typeof(Define_LDH.GlobalUI));
            foreach (string typeString in globalUIs)
            {
                GameObject go = Util_LDH.Instantiate<GameObject>(
                    Path.Combine(_globalUIPrefabFolder, typeString), UIRoot.transform);
                
                // 타입 얻기 (주의: 네임스페이스 포함 필요)
                Type uiType = Type.GetType($"GameUI.{typeString}"); // 예: GameUI.InventoryUI
                
                if (uiType == null)
                {
                    Debug.LogError($"[{GetType().Name}] 타입을 찾을 수 없습니다: {typeString}");
                    continue;
                }

                UI_Base ui = Util_LDH.GetOrAddComponent(go, uiType) as UI_Base;
                _globalUIDict.Add(uiType, ui);
                
                ui.gameObject.SetActive(false);
            }
            
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

        public UI_Base GetGlobalUI(Define_LDH.GlobalUI globalUIType)
        {
            Type type = Type.GetType($"GameUI.{globalUIType}");
            return _globalUIDict.GetValueOrDefault(type);
        }

        public T GetGlobalUI<T>() where T : UI_Base
        {
            return _globalUIDict.TryGetValue(typeof(T), out var ui) ? ui as T : null;
        }

        // public void ToggleGlobalUI<T>(bool isActive) where T : UI_Base
        // {
        //     if (_globalUIDict.TryGetValue(typeof(T), out var ui))
        //     {
        //         ui.gameObject.SetActive(isActive);
        //     }
        // }

        public UI_Base ShowGlobalUI(Define_LDH.GlobalUI globalUIType)
        {
            Type type = Type.GetType($"GameUI.{globalUIType}");

            if (!_globalUIDict.TryGetValue(type, out var ui)) return null;
            
            SetCanvas(ui.gameObject, true);
            
            if (ui is UI_Popup popup)
                _popupStack.Push(popup);

            ui.gameObject.SetActive(true);
            
            return ui;
        }


        public void CloseGlobalUI(Define_LDH.GlobalUI globalUIType)
        {
            Type type = Type.GetType($"GameUI.{globalUIType}");

            if (!_globalUIDict.TryGetValue(type, out var ui)) return;


            if (ui is UI_Popup popup)
            {
                if (_popupStack.Count == 0)
                    return;
               
                if (_popupStack.Peek() != popup)
                {
                    Debug.Log($"[{GetType().Name}] pop up이 가장 위에 있는 팝업이 아닙니다.");
                    return;
                }
                
                _popupStack.Pop();
              
            }
            
            ui.gameObject.SetActive(false);

            _order--;
           
            
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
                Debug.Log($"[{GetType().Name}] pop up이 가장 위에 있는 팝업이 아닙니다.");
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