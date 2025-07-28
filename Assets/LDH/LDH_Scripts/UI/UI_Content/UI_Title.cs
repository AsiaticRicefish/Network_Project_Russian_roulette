using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Video;
using Utils;

namespace GameUI
{
    /// <summary>
    /// 타이틀 화면 제어
    /// </summary>
    public class UI_Title : MonoBehaviour
    {
        [Header("Title Video")] 
        [SerializeField] private VideoPlayer _videoPlayer;
        [SerializeField] private float _videoStartDelay = 0.5f;


        [Header("Panels")] 
        [SerializeField] private GameObject _titlePanel;
        [SerializeField] private Animator _anim_signupPanel;
        [SerializeField] private Animator _anim_loginPanel;
        // private ModalWindowManager _signupPanel;
        // private ModalWindowManager _loginPanel;


        [Header("Buttons")] 
        [SerializeField] private GameObject _signupButton;
        [SerializeField] private GameObject _loginButton;


        [Header("Panel Animation Config")]
        [SerializeField] private float _showTitlePanelDelay = 0.3f;
        private WaitForSeconds _titlePanelWait;

        // Animator 해시값
        private static int panelFadeInHash = Animator.StringToHash("Fade-in");
        private static int panelFadeOutHash = Animator.StringToHash("Fade-out");


        private void Awake()
        {
            Init();
            Subscribe();
        }

        private void Start()
        {
            StartCoroutine(PlayVideoAfterDelay());
        }

        
        /// <summary>
        /// 초기 설정: 타이틀 패널 숨김 및 대기 시간 초기화
        /// </summary>
        private void Init()
        {
            _titlePanel.SetActive(false);
            _titlePanelWait = new WaitForSeconds(_showTitlePanelDelay);

            // _signupPanel = _anim_signupPanel.GetComponent<ModalWindowManager>();
            // _loginPanel = _anim_loginPanel.GetComponent<ModalWindowManager>();
        }

        
        /// <summary>
        /// 버튼, 영상 이벤트 바인딩
        /// </summary>
        private void Subscribe()
        {
            // video play 종료 시 타이틀 패널 활성화
            _videoPlayer.loopPointReached += ShowTitle;

            // sign up button
            UI_Base.BindUIEvent(_signupButton, ShowSignupPanel, Define_LDH.UIEvent.Click);

            // log in button
            UI_Base.BindUIEvent(_loginButton, ShowLoginPanel, Define_LDH.UIEvent.Click);
        }

        #region Video

        
        /// <summary>
        /// 약간의 지연 후 영상 재생 시작
        /// </summary>
        private IEnumerator PlayVideoAfterDelay()
        {
            yield return new WaitForSeconds(_videoStartDelay);
            _videoPlayer.Play();
        }

        #endregion


        #region Show UI/Panel

        /// <summary>
        /// 영상 종료 시 타이틀 패널 표시
        /// </summary>
        private void ShowTitle(VideoPlayer videoPlayer)
        {
            _titlePanel.SetActive(true);
        }

        /// <summary>
        /// 회원가입 패널 열기
        /// </summary>
        private void ShowSignupPanel(PointerEventData eventData)
        {
            _titlePanel.SetActive(false);
            _anim_signupPanel.Play(panelFadeInHash);
        }

        /// <summary>
        /// 로그인 패널 열기
        /// </summary>
        private void ShowLoginPanel(PointerEventData eventData)
        {
            _titlePanel.SetActive(false);
            _anim_loginPanel.Play(panelFadeInHash);
        }

        #endregion

        #region ClosePanel

        /// <summary>
        /// 회원가입 패널 닫기 후 타이틀 복귀
        /// </summary>
        public void CloseSignupPanel()
        {
            StartCoroutine(ClosePanelAndShowTitle(_anim_signupPanel));
        }
        
        /// <summary>
        /// 로그인 패널 닫기 후 타이틀 복귀
        /// </summary>
        public void CloseLoginPanel()
        {
            StartCoroutine(ClosePanelAndShowTitle(_anim_loginPanel));
        }

        /// <summary>
        /// 닫기 애니메이션 재생 후 타이틀 패널 활성화
        /// </summary>
        private IEnumerator ClosePanelAndShowTitle(Animator panelAnimator)
        {
            panelAnimator.Play(panelFadeOutHash);
            Debug.Log("delay");
            yield return _titlePanelWait;
            Debug.Log("afterdelay");
            _titlePanel.SetActive(true);
        }
        
        

        #endregion
    }
}