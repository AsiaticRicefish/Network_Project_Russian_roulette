using ETC;
using Managers;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Utils;
using UIManager = Managers.UIManager;

namespace GameUI
{
    public class UI_Setting : UI_Popup
    {
        public ModalWindowManager settingModal;
        [SerializeField] private UI_VolumeSetting _volumeSetting;

        [SerializeField] private Button _leaveRoomButton;
        [SerializeField] private Button _quitButton;

        public bool isInitialized = false;

        protected override void Init()
        {
            base.Init();

            //이벤트 구독
            _leaveRoomButton.onClick.AddListener(LeaveRoom);
            _quitButton.onClick.AddListener(Quit);
            SceneManager.sceneLoaded += SetLeaveRoomButtonFunc;
        }


        private void OnEnable()
        {
            Show();
        }

        private void SetLeaveRoomButtonFunc(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == Define_LDH.GetSceneName(Define_LDH.SceneName.CutScene))
            {
                //컷씬일 때는 버튼 자체를 비활성화
                _quitButton.gameObject.SetActive(false);
                _leaveRoomButton.gameObject.SetActive(false);
            }
            else
            {
                _quitButton.gameObject.SetActive(true);
                

                if (scene.name == Define_LDH.GetSceneName(Define_LDH.SceneName.InGame))
                    _leaveRoomButton.gameObject.SetActive(true);
                else
                    _leaveRoomButton.gameObject.SetActive(false);
                
            }
        }


        public override void Show()
        {
            settingModal.OpenWindow();
        }

        public override void Close()
        {
            StartCoroutine(CloseWithDelay());
        }

        private IEnumerator CloseWithDelay()
        {
            settingModal.CloseWindow();

            if (isInitialized)
                yield return new WaitForSeconds(0.5f);

            Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_Setting);
        }

        private void LeaveRoom()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                if (InGameManager.Instance != null && !InGameManager.Instance.IsGameOver)

                    //방 나가기
                    PhotonNetwork.LeaveRoom(false);
                Close();
            }
        }


        /// <summary>
        /// 게임 종료
        /// </summary>
        private void Quit()
        {
#if UNITY_EDITOR
            // 에디터에서는 에디터 플레이 모드 종료
            UnityEditor.EditorApplication.isPlaying = false;
#else
            // 빌드된 게임에서는 실제 게임 종료
            Application.Quit();
#endif
        }
    }
}