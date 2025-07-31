using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_Room : MonoBehaviour
    {
        [Header("UI Elements")]
        //--- 텍스트 ----//
        [SerializeField]
        private TMP_Text _roomNameText;

        [SerializeField] private TMP_Text _roomCodeText;

        //--- 버튼 ----//
        [SerializeField] private UI_ObservableButton _startButton;
        [SerializeField] private Button _leaveButton;

        //--- 플레이어 패널 ----//
        [SerializeField] private PlayerPanel _hostPanel;
        [SerializeField] private PlayerPanel _clientPanel;

        private ButtonManager _startButtonManager;
       

        [Header("Event")] public Action OnClickStartButton;
        public Action OnClickLeaveButton;

        private void Awake()
        {
            _startButtonManager = _startButton.GetComponent<ButtonManager>();
        }

        private void Start() => Subscribe();

        private void Subscribe()
        {
            _startButton.button.onClick.AddListener(() => OnClickStartButton?.Invoke());

            _leaveButton.onClick.AddListener(() => OnClickLeaveButton?.Invoke());
        }


        public void Init(Action onStart, Action onLeave)
        {
            //초기화
            ResetAllSetting();
            //버튼 이벤트 설정
            SetStartButton();
            if(onStart!=null)
                OnClickStartButton += onStart;
            OnClickLeaveButton += onLeave;

            var roomInfo = PhotonNetwork.CurrentRoom;

            //룸 이름, 룸 코드 설정
            _roomNameText.text = roomInfo?.CustomProperties["userRoomName"] as string;
            _roomCodeText.text = $"#{roomInfo?.CustomProperties["roomCode"] as string}";
        }

        #region Button Control

        private void SetStartButton()
        {
            if (PhotonNetwork.IsMasterClient)
            {
                // host인 경우 start button으로 기능
                UpdateStartButtonText("START");
                
            }

            else
            {
                //host 아닌 경우 ready로 기능
                UpdateStartButtonText("READY");
                OnClickStartButton += _clientPanel.ReadyButtonClick;
                
                _startButton.SetInteractable(true);
            }
        }

        #endregion

        #region Player Panel

        public void SetPlayerPanel(Player player)
        {
            if (player.IsMasterClient)
            {
                _hostPanel.SetData(player);
                
            }

            else
            {
                _clientPanel.SetData(player);
               
            }
        }

        #endregion

        public void ResetPanel(Player player)
        {
            if (player.IsMasterClient)
                _hostPanel.Reset();
            else
                _clientPanel.Reset();
        }

        public void ResetAllSetting()
        {
            OnClickLeaveButton = null;
            OnClickStartButton = null;
            _startButton.SetInteractable(false);
            _hostPanel.Reset();
            _clientPanel.Reset();
        }


        public void UpdateStartButtonState(bool isAllReady)
        {
            if (!PhotonNetwork.IsMasterClient) return;

            _startButton.SetInteractable(PhotonNetwork.CurrentRoom.PlayerCount == 2 && isAllReady);
        }

        public void UpdateReadyUI(Player player)
        {
            var _localPanel = player.IsMasterClient ? _hostPanel : _clientPanel;
            _localPanel.ReadyCheck(player);
        }

        public void UpdateStartButtonText(string text)
        {
            _startButtonManager.buttonText = text;
            _startButtonManager.UpdateUI();
        }
    }
}