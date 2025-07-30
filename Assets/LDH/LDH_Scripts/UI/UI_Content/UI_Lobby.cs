using Michsky.UI.ModernUIPack;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_Lobby : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] private NetworkManager _networkManager;
      
        
        
        [Header("Create Room")]
        [SerializeField] private ModalWindowManager createRoomPanel;
        [SerializeField] private Button createRoomButton;
        
        [Header("Join By Code")]
        [SerializeField] private ModalWindowManager joinRoomPanel;
        [SerializeField] private Button joinByCodeButton;
       
        [Header("Join")]
        [SerializeField] private UI_ObservableButton joinButton;

        
        
        public ModalWindowManager CreateRoomPanel => createRoomPanel;
        public ModalWindowManager JoinRoomPanel => joinRoomPanel;
        
       
        private RoomList _selectedRoom;
        public RoomList SelectedRoom => _selectedRoom;
        
        private void Start()
        {
            _selectedRoom = null;
            joinButton.SetInteractable(false);
            joinButton.button.onClick.AddListener(OnClickJoin);
            
            createRoomButton.onClick.AddListener(createRoomPanel.OpenWindow);
            joinByCodeButton.onClick.AddListener(joinRoomPanel.OpenWindow);
        }
        
        public void OnRoomSelected(RoomList selected)
        {
            _selectedRoom = selected;
            foreach (var roomListObj in _networkManager.RoomList)
            {
                RoomList room = roomListObj.GetComponent<RoomList>();
                
                room.SetSelected(room == selected);
            }

            joinButton.SetInteractable(selected != null);
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