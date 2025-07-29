using Managers;
using Michsky.UI.ModernUIPack;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;
using UIManager = Managers.UIManager;

namespace GameUI
{
    public class UI_Login : MonoBehaviour
    {
        //------ temp variable for test ----- //
        [Header("Test Login Result (Simulated)")]
        [SerializeField] private bool _isSuccess;

        //------ firebase 연동시 해당 변수는 삭제, 각 로그인 실패 상황에 따라 적절한 title, message로 설정해야 함 ----- //
        [Header("Fail Modal (Test)")]
        [SerializeField] private Define_LDH.NotifyType failType;
        [SerializeField] private string _failTitle;
        [SerializeField] private string _failMessage;
        
        [Header("Modal Position Offset")]
        [SerializeField] private Vector2 _offset;

        
        [Header("UI Contents")] 
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        
        
        private ModalWindowManager _loginModal;


        private void Awake() => Init();
        private void Start() => Subscribe();
        
        private void Init()
        {
            _loginModal = GetComponent<ModalWindowManager>();
        }

        private void Subscribe()
        {
            _loginModal.onConfirm.AddListener(OnLoginConfirm);
            _loginModal.onCancel.AddListener(Clear);
        }

        
        /// <summary>
        /// 로그인 결과
        /// </summary>
        public void OnLoginConfirm()
        {
            //todo: 로그인 로직과 연결 필요

            if (_isSuccess)
            {
                Debug.Log($"[{GetType().Name}] 로그인에 성공하여 씬 이동합니다.");
                SceneManager.LoadSceneAsync("Lobby");
            }
            else
            {
                Debug.Log($"[{GetType().Name}] 로그인 실패");
                ShowModal(failType, _failTitle, _failMessage);
            }
        }


        /// <summary>
        /// 모달 생성 및 표시
        /// </summary>
        private void ShowModal(Define_LDH.NotifyType type, string title, string description)
        {
            UI_Modal modal = Manager.UI.SpawnPopupUI<UI_Modal>("UI_SlidingModal");
            
            //content 데이터 설정
            modal.SetContent(type, title, description);

            RectTransform modalRect = modal.GetComponent<RectTransform>();
            
            //rect transform 설정
            Util_LDH.SetRightBottom(modalRect, modalRect.rect.size, new Vector2(_offset.x, _offset.y));
            
            //모달 보이게
            modal.Show();
        }
        
        
        /// <summary>
        /// 입력값 초기화
        /// </summary>
        public void Clear()
        {
            _emailField.text
                = _passwordField.text
                    = "";
            
            _emailField.gameObject.SetActive(false);
            _passwordField.gameObject.SetActive(false);
            
            _emailField.gameObject.SetActive(true);
            _passwordField.gameObject.SetActive(true);
            
        }
        
    }
}