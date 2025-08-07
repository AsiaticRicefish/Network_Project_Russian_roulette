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
        
        
        public bool isInitialized = false;
        
        protected override void Init()
        {
            base.Init();
            _leaveRoomButton.onClick.AddListener(LeaveRoom);
            SceneManager.sceneLoaded += SetLeaveRoomButtonActive;
        }
        

        private void OnEnable()
        {
           Show();
        }

        private void SetLeaveRoomButtonActive(Scene scene, LoadSceneMode mode)
        {
            if (scene.name == "PMS_InGame")
            {
                _leaveRoomButton.gameObject.SetActive(true);
                
            }
            else
            {
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
            
            if(isInitialized)
                yield return new WaitForSeconds(0.5f);
            
            Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_Setting);
        }
        
        private void LeaveRoom()
        {
            if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
            {
                //방 나가기
                PhotonNetwork.LeaveRoom(false);
                Close();
            }
        }
        
    }
}