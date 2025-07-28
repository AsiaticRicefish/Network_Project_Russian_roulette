using GameUI;
using System;
using System.Collections.Generic;
using System.IO;
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

        
        //style scriptable
        public NotifyStyleTable notifyStyle { get; private set; }
        
        [SerializeField] private string _notifyStyleTablePath = "Data/NotifyStyleTable";

        
        
        
        //전역 UI 관리
        //전역으로 사용되는 UI
        private Dictionary<Type, UI_Base> _globalUIDict = new();

        
        
        

        //Popup prefab path Folder Path
        [SerializeField] private string _popupPrefabFolder = "Prefabs/UI/Popup";
        [SerializeField] private string _globalUIPrefabFolder = "Prefabs/UI/Global";

        private void Awake() => Init();

        private void Init()
        {
            SingletonInit(); //싱글톤 초기화
            
            LoadNotifyStyleTable();
            
            InitUIRoot(); //UI Root를 생성

            Clear(); //Popup Stack 초기화

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
        
        private void LoadNotifyStyleTable()
        {
            notifyStyle = Resources.Load<NotifyStyleTable>(_notifyStyleTablePath);
            notifyStyle.Init();
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

        public T SpawnPopupUI<T>(string name = null) where T : UI_Popup
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
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        #endregion


        public void Clear()
        {
            CloseAllPopupUI();
        }

        #region RectTransform Control

        /// <summary>
        /// 주어진 RectTransform의 anchor, pivot, anchoredPosition, sizeDelta를 설정합니다.
        /// basePosition은 anchoredPosition의 기준 위치이며, offset이 있다면 추가됩니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="anchorMin">Anchor Min 값</param>
        /// <param name="anchorMax">Anchor Max 값</param>
        /// <param name="pivot">Pivot 기준</param>
        /// <param name="basePosition">기준 위치 (anchoredPosition)</param>
        /// <param name="sizeDelta">UI 크기 (width, height). 생략 시 기존 값 유지</param>
        /// <param name="offset">basePosition에 추가로 더해질 오프셋</param>
        public static void SetRectTransform(
            RectTransform rect,
            Vector2 anchorMin,
            Vector2 anchorMax,
            Vector2 pivot,
            Vector2 basePosition,
            Vector2? sizeDelta = null,
            Vector2? offset = null
        )
        {
            if (rect == null) return;

            rect.anchorMin = anchorMin;
            rect.anchorMax = anchorMax;
            rect.pivot = pivot;

            // 위치 = 기준 위치 + 오프셋
            rect.anchoredPosition = basePosition + (offset ?? Vector2.zero);

            if (sizeDelta.HasValue)
                rect.sizeDelta = sizeDelta.Value;
        }

        /// <summary>
        /// 부모 영역 전체를 가득 채우는 Full Stretch UI로 설정합니다.
        /// (anchorMin = (0,0), anchorMax = (1,1), pivot = center)
        /// sizeDelta는 (0,0)으로 설정됩니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="offset">anchoredPosition에 적용할 오프셋</param>
        public static void SetFullStretch(RectTransform rect, Vector2? offset = null)
        {
            SetRectTransform(rect, Vector2.zero, Vector2.one, new Vector2(0.5f, 0.5f), Vector2.zero, Vector2.zero,
                offset);
        }

        /// <summary>
        /// 화면 정중앙 기준으로 위치시키며, 명시한 크기로 설정합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">기준 위치에서의 오프셋</param>
        public static void SetCenter(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 center = new Vector2(0.5f, 0.5f);
            SetRectTransform(rect, center, center, center, Vector2.zero, size, offset);
        }
        
        
        /// <summary>
        /// 오른쪽 위 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">오른쪽 위 기준 위치에서의 오프셋</param>
        public static void SetRightTop(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(1f, 1f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        
        /// <summary>
        /// 오른쪽 아래 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">오른쪽 아래 기준 위치에서의 오프셋</param>
        public static void SetRightBottom(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(1f, 0f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        

        /// <summary>
        /// 왼쪽 위 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">왼쪽 위 기준 위치에서의 오프셋</param>
        public static void SetLeftTop(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(0f, 1f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }

        
        /// <summary>
        /// 왼쪽 아래 모서리를 기준으로 UI를 배치합니다.
        /// </summary>
        /// <param name="rect">대상 RectTransform</param>
        /// <param name="size">UI의 크기 (width, height)</param>
        /// <param name="offset">왼쪽 아래 기준 위치에서의 오프셋</param>
        public static void SetLeftBottom(RectTransform rect, Vector2 size, Vector2? offset = null)
        {
            Vector2 pos = new Vector2(0f, 0f);
            SetRectTransform(rect, pos, pos, pos, Vector2.zero, size, offset);
        }
        
        #endregion
    }
}