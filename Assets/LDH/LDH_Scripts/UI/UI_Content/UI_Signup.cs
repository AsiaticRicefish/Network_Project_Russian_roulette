using Managers;
using Michsky.UI.ModernUIPack;
using System;
using Test;
using TMPro;
using UnityEngine;
using Utils;
using UIManager = Managers.UIManager;

namespace GameUI

{
    public class UI_Signup : MonoBehaviour
    {
        //------ temp variable for test ----- //
        [Header("Test Signup Result (Simulated)")]
        [SerializeField] private bool _signupSuccess;
        
        //------ firebase 연동시 해당 변수는 삭제, 각 회원가입 성공/실패 상황에 따라 적절한 title, message로 설정해야 함 ----- //
        [Header("Fail Modal (Test)")]
        [SerializeField] private Define_LDH.NotifyType failType;
        [SerializeField] private string _failTitle;
        [SerializeField] private string _failMessage;
        
        [Header("Success Modal (Test)")]
        [SerializeField] private string _successTitle;
        [SerializeField] private string _successMessage;

        [Header("Email Check Message Texts (Test)")]
        private string emailCheckSucessTitle = "Valid Email";
        private string emailCheckFailTitle = "Invalid Email";
        private string emailCheckSuccessMessage = "Your email is valid.";
        private string emailCheckFailMessage = "Your email is invalid.";
        
        
        [Header("Modal Position Offset")]        
        [SerializeField] private Vector2 _offset;
        
        
        [Header("UI Contents")] 
        [SerializeField] private TMP_InputField _nameField;
        [SerializeField] private TMP_InputField _emailField;
        [SerializeField] private TMP_InputField _passwordField;
        [SerializeField] private TMP_InputField _confirmPasswordField;
        [SerializeField] private UI_FeedbackButton _emailCheckButton;
        
        
        //todo: 나중에 firebase manager로 교체해야함
        [Header("Temp Firebase Stub")]
        [SerializeField] private TempFirebaseManager _firebaseManager;
        
        
        private ModalWindowManager _signupModal;
        
        
        
        
        private void Awake() => Init();
        private void Start() => Subscribe();
        
        private void Init()
        {
            _signupModal = GetComponent<ModalWindowManager>();
        }

        private void Subscribe()
        {
            _signupModal.onConfirm.AddListener(OnSignupConfirm);
            _signupModal.onCancel.AddListener(Clear);
            _emailCheckButton.onSuccessEvent.AddListener(ShowEmailCheckSuccess);
            _emailCheckButton.onFailEvent.AddListener(ShowEmailCheckFail);
            
            
            // ---  테스트 Firebase 시뮬레이션  ---- //
            //todo : firebase 연동시 수정
            UI_Base.BindUIEvent(_emailCheckButton.gameObject, (_) =>  _firebaseManager.SimulateEmailCheck());
            
            //todo : firebase 연동시 수정
            _firebaseManager.OnCheckEmailResult += _emailCheckButton.ShowResult;  // 비동기 결과 수신 이벤트 구독
        }
        
        
        /// <summary>
        /// 회원가입 결과
        /// </summary>
        public void OnSignupConfirm()
        {
            //todo: 회원가입 로직과 연결
            switch (_signupSuccess)
            {
                case true:
                    Debug.Log($"[{GetType().Name}] 회원가입 성공");
                    ShowModal(Define_LDH.NotifyType.Check, _successTitle, _successMessage);
                    _signupModal.onCancel?.Invoke();
                    break;
                case false:
                    Debug.Log($"[{GetType().Name}] 회원가입 실패");
                    ShowModal(failType, _failTitle, _failMessage);
                    break;
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


        #region Email Check

        /// <summary>
        /// 이메일 유효성 검사 성공
        /// </summary>
        public void ShowEmailCheckSuccess()
        {
            ShowModal(Define_LDH.NotifyType.Check, emailCheckSucessTitle, emailCheckSuccessMessage );
        }

        /// <summary>
        /// 이메일 유효성 검사 실패
        /// </summary>
        public void ShowEmailCheckFail()
        {
            ShowModal(Define_LDH.NotifyType.Error, emailCheckFailTitle, emailCheckFailTitle );
        }
        
        #endregion
        

    

        /// <summary>
        /// 입력값 초기화
        /// </summary>
        public void Clear()
        {
            _emailField.text
                = _nameField.text
                    = _passwordField.text
                        = _confirmPasswordField.text
                            = "";
            
            // 강제로 비활성 -> 활성화하여 InputField 초기화
            _emailField.gameObject.SetActive(false);
            _nameField.gameObject.SetActive(false);
            _confirmPasswordField.gameObject.SetActive(false);
            _passwordField.gameObject.SetActive(false);
            
            _emailField.gameObject.SetActive(true);
            _nameField.gameObject.SetActive(true);
            _confirmPasswordField.gameObject.SetActive(true);
            _passwordField.gameObject.SetActive(true);
            
            // feedback button 초기화
            _emailCheckButton.Clear();
        }
    }
}