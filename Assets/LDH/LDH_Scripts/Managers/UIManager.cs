using GameUI;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Utils;

namespace Managers
{
    /// <summary>
    /// 게임 내 UI 전체를 총괄하는 매니저.
    /// 전역 UI, 팝업 UI, 캔버스 정렬 및 스타일 테이블 초기화 등을 담당한다.
    /// </summary>
    public class UIManager : DesignPattern.Singleton<UIManager>
    {
        //---- Stack 관리 ----//
        [SerializeField] private int _order = 10;        // UI 정렬 순서 제어

        public int order
        {
            get => _order;
            set
            {
                Debug.Log($"canvas order value 변경 - new value : {value}");
                _order = value;
            }
        }                                                // 용
        private Stack<UI_Popup> _popupStack = new();    // 팝업 UI Stack
        
        //---- 전역 UI 관리 (Global UI) ----//
        private Dictionary<Type, UI_Base> _globalUIDict = new(); // 전역 UI Dictionary (Type 기준 캐싱)
        
        //---- UI Root 오브젝트 ---- //
        public static GameObject UIRoot { get; private set; }
        
        //---- Notify 스타일 ScriptableObject ----//
        public NotifyStyleTable notifyStyle { get; private set; }
        
        
        [Header("Path Setting")]
        [SerializeField] private string _notifyStyleTablePath = "Data/NotifyStyleTable";
        [SerializeField] private string _popupPrefabFolder = "Prefabs/UI/Popup";
        [SerializeField] private string _globalUIPrefabFolder = "Prefabs/UI/Global";
        
        
        

        private void Awake() => Init();

        // UI 매니저 초기화
        private void Init()
        {
            SingletonInit(); //싱글톤 초기화
            
            InitUIRoot(); //UI Root를 생성
            
            LoadNotifyStyleTable(); // 스타일 테이블 로드

            CloseAllPopupUI(); //Popup Stack 초기화

            InitGlobalUIs();   // 전역 UI 생성 및 등록
            
            
        }

        #region Initialize
        
        /// <summary>
        /// UI 루트 오브젝트 생성 및 초기화
        /// </summary>
        private void InitUIRoot()
        {
            if (UIRoot != null) return;
            UIRoot = new GameObject("@UIRoot");
            DontDestroyOnLoad(UIRoot);   // 파괴 방지
        }

        /// <summary>
        /// 알림 스타일 ScriptableObject 로드 및 초기화
        /// </summary>
        private void LoadNotifyStyleTable()
        {
            notifyStyle = Resources.Load<NotifyStyleTable>(_notifyStyleTablePath);
            notifyStyle.Init();  // 내부 초기화
        }

        /// <summary>
        /// 전역 UI 프리팹들을 로드 및 초기화
        /// </summary>
        private void InitGlobalUIs()
        {
            foreach (Define_LDH.GlobalUI uiEnum in Enum.GetValues(typeof(Define_LDH.GlobalUI)))
            {
                // 1. Enum → 이름 → 경로 변환
                string globalUIName = uiEnum.ToString();
                string fullPath = Path.Combine(_globalUIPrefabFolder, globalUIName);
                
                // 2. 프리팹 인스턴스화
                GameObject go = Util_LDH.Instantiate<GameObject>(fullPath, UIRoot.transform);
                
                // 3. Type 얻기 (주의: 네임스페이스 포함 문자열 필요)
                Type uiType = GetUIType(globalUIName);
                
                if (uiType == null)
                {
                    Debug.LogError($"[{GetType().Name}] 타입을 찾을 수 없습니다: {globalUIName}");
                    continue;
                }
                
                // 4. 컴포넌트 캐싱 및 비활성화
                UI_Base ui = Util_LDH.GetOrAddComponent(go, uiType) as UI_Base;
                _globalUIDict.Add(uiType, ui);

                ui.Close();
            }
            
        }
        
        /// <summary>
        /// UI 타입 문자열로부터 Type 객체 반환
        /// </summary>
        private Type GetUIType(string name) => Type.GetType($"GameUI.{name}");



        #endregion

      
        

        #region Canvas Setting

        /// <summary>
        /// 지정한 오브젝트에 Canvas 컴포넌트를 설정하고 정렬 순서를 부여합니다.
        /// </summary>
        public void SetCanvas(GameObject go, bool sort = true)
        {
            Canvas canvas = Util_LDH.GetOrAddComponent<Canvas>(go);
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.overrideSorting = true;

            canvas.sortingOrder = sort ? order++ : 0;
        }

        #endregion

        #region Global UI

        
        /// <summary>
        /// Enum 기반으로 전역 UI 인스턴스를 가져옵니다.
        /// </summary>
        public UI_Base GetGlobalUI(Define_LDH.GlobalUI uiEnum)
        {
            Type type = GetUIType(uiEnum.ToString());
            return _globalUIDict.GetValueOrDefault(type);
        }

        /// <summary>
        /// 제네릭 타입으로 전역 UI를 가져옵니다.
        /// </summary>
        public T GetGlobalUI<T>() where T : UI_Base
        {
            return _globalUIDict.TryGetValue(typeof(T), out var ui) ? ui as T : null;
        }

        /// <summary>
        /// 전역 UI를 활성화합니다.
        /// 팝업일 경우 Stack에 Push합니다.
        /// </summary>
        public UI_Base ShowGlobalUI(Define_LDH.GlobalUI uiEnum)
        {
            Type type = GetUIType(uiEnum.ToString());
            if (!_globalUIDict.TryGetValue(type, out var ui)) return null;
            
            //이미 켜져 있으면 그대로 둔다.
            if(ui.gameObject.activeSelf) return ui;
            
            SetCanvas(ui.gameObject);
            
            // 팝업일 경우 Stack에 등록
            if (ui is UI_Popup popup)
                _popupStack.Push(popup);

            ui.gameObject.SetActive(true);
            return ui;
        }
        
        
        /// <summary>
        /// 전역 UI를 비활성화합니다. (팝업이면 Stack에서 Pop)
        /// </summary>
        public void CloseGlobalUI(Define_LDH.GlobalUI uiEnum)
        {
            Type type = GetUIType(uiEnum.ToString());
            if (!_globalUIDict.TryGetValue(type, out var ui)) return;
            
            //이미 꺼져 있으면 그대로 둔다.
            if(!ui.gameObject.activeSelf) return;

            // 팝업일 경우 Stack 검사 후 Pop
            if (ui is UI_Popup popup && _popupStack.Count > 0)
            {
                if (_popupStack.Peek() != popup)
                {
                    //Debug.LogWarning($"[{GetType().Name}] 닫으려는 팝업이 최상단 팝업이 아닙니다.");
                    return;
                }
                _popupStack.Pop();
            }

            ui.gameObject.SetActive(false);
            order--;
        }

        #endregion
        

        #region Popup UI

        /// <summary>
        /// 팝업 UI를 생성하고 Stack에 등록합니다. 활성화는 직접 Show를 호출해야 한다.
        /// </summary>
        public T SpawnPopupUI<T>(string name = null) where T : UI_Popup
        {
            name ??= typeof(T).Name;
            T popup = Util_LDH.Instantiate<T>(Path.Combine(_popupPrefabFolder, name), UIRoot.transform);
            _popupStack.Push(popup);
            return popup;
        }
        
        /// <summary>
        /// 특정 팝업을 닫습니다. (최상단일 때만 가능)
        /// </summary>
        public void ClosePopupUI(UI_Popup popup)
        {
            if (_popupStack.Count == 0 || _popupStack.Peek() != popup)
            {
                Debug.LogWarning($"[{GetType().Name}] 닫으려는 팝업이 최상단 팝업이 아닙니다.");
                return;
            }

            ClosePopupUI();
        }

        /// <summary>
        /// 최상단 팝업을 닫습니다.
        /// </summary>
        public void ClosePopupUI()
        {
            // Stack 비어있을 경우 리턴
            if (_popupStack.Count == 0)
                return;
            
            // 최상단 팝업 Pop 후 제거
            UI_Popup popup = _popupStack.Pop();
            Destroy(popup.gameObject);
            popup = null;
            
            // 정렬 순서 감소
            order--;
        }

        /// <summary>
        /// 모든 팝업을 제거합니다.
        /// </summary>
        public void CloseAllPopupUI()
        {
            while (_popupStack.Count > 0)
                ClosePopupUI();
        }

        #endregion



        #region API

        public void ShowNotifyModal(Define_LDH.NotifyType notifyType, string title, string description)
        {
            var modal = SpawnPopupUI<UI_Modal>("UI_SlidingModal");
            modal.SetContent(notifyType,title, description);
            
            modal.Show();
        }

        public void ShowNotifyModal(MessageEntity messageEntity)
        {
            ShowNotifyModal(messageEntity.NotifyType, messageEntity.Title, messageEntity.Description);
        }

        #endregion
    }
}