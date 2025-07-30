using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_Lobby : MonoBehaviour
    {
        [SerializeField] private NetworkManager _networkManager;

        [SerializeField] private Button createRoomButton;
        
        [SerializeField] private ModalWindowManager createRoomPanel;
        [SerializeField] private ModalWindowManager joinRoomPanel;

        public ModalWindowManager CreateRoomPanel => createRoomPanel;
        public ModalWindowManager JoinRoomPanel => joinRoomPanel;
        
        
        [SerializeField] private Button joinButton;
        private RoomList _selectedRoom;
        
        private void Start()
        {
            _selectedRoom = null;
            joinButton.interactable = false;
            joinButton.onClick.AddListener(OnClickJoin);
            
            createRoomButton.onClick.AddListener(createRoomPanel.OpenWindow);
        }
        
        public void OnRoomSelected(RoomList selected)
        {
            _selectedRoom = selected;
            foreach (var roomListObj in _networkManager.RoomList)
            {
                RoomList room = roomListObj.GetComponent<RoomList>();
                
                room.SetSelected(room == selected);
            }

            joinButton.interactable = true;
        }
        
        
        private void OnClickJoin()
        {
            if (_selectedRoom != null && Photon.Pun.PhotonNetwork.InLobby)
            {
                _selectedRoom.JoinRoom();
            }
        }


        public void ClearInputField(TMP_InputField inputField)
        {
            inputField.text = "";
            inputField.gameObject.SetActive(false);
            inputField.gameObject.SetActive(true);
        }
    }
}