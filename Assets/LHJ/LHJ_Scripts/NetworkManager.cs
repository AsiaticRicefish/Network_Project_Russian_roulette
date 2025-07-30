using ExitGames.Client.Photon;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using GameUI;
using Managers;
using Michsky.UI.ModernUIPack;
using System;
using System.Linq;
using Utils;
using static Utils.Define_LDH;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [Header("Loading")]
    [SerializeField] private GameObject loadingPanel;
    
    [Header("Nickname")]
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private Button nicknameAdmitButton;
    
    [Header("User Label")] 
    [SerializeField] private GameObject userLabel;

    [Header("Lobby")]
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Button roomNameAdmitButton;
    [SerializeField] private GameObject roomListPrefabs;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private TMP_InputField codeInputField;
    [SerializeField] private Button codeJoinButton;


    
    [Header("Room")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private RoomManager roomManager;
    private Dictionary<string, GameObject> roomList = new Dictionary<string, GameObject>();
    public IReadOnlyList<GameObject> RoomList => roomList.Values.ToList();
    
    
    #region ui scripts reference (private 접근자)
    private UI_Nickname _uiNicknamePanel;
    private UI_Lobby _uiLobby;
    private ModalWindowManager _createRoomManager;
    #endregion

    private void Awake() => Init();
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   // 마스터 서버 접속
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);
        roomNameAdmitButton.onClick.AddListener(CreateRoom);
        codeJoinButton.onClick.AddListener(JoinRoomByCode);
    }

    #region Init
    
    private void Init()
    {
        _uiNicknamePanel = nicknamePanel.GetComponent<UI_Nickname>();
        _uiLobby = lobbyPanel.GetComponent<UI_Lobby>();
        
        InitUIVisibiity();
    }

    private void InitUIVisibiity()
    {
        //loading 활성화
        loadingPanel.SetActive(true);
        //lobby 비활성화
        lobbyPanel.SetActive(false);
        //user label 비활성화
        userLabel.SetActive(false);
        //nickname 설정 패널 invisible
        _uiNicknamePanel.SetActive(false);
        //create room 패널 invisible
        _uiLobby.CreateRoomPanel.CloseWindow();
        //join room by code 패널 invisible
        _uiLobby.JoinRoomPanel.CloseWindow();
        //room 비활성화
        roomPanel.SetActive(false);
    }
    #endregion

    #region Connect
    // 마스터 서버 연결
    public override void OnConnectedToMaster()
    {
        if (loadingPanel.activeSelf)
        {
            Debug.Log("Onconnected to master : loadingpanel active is true");
            loadingPanel.SetActive(false);
            _uiNicknamePanel.SetActive(true);     // 닉네임 panel 활성화(open window)
        }
        else
        {
            Debug.Log("Onconnected to master : loadingpanel active is false");
            PhotonNetwork.JoinLobby();  // 로비로 진입
            roomPanel.SetActive(false);
        }
        
        

    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.ConnectUsingSettings(); // 재접속
    }
    

    #endregion
    
    
    #region NickName

    // 닉네임 설정 및 로비진입
    public void NicknameAdmit()
    {
        if (string.IsNullOrWhiteSpace(nicknameField.text))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NicknameError]);     // ui modal
            return;
        }
        
        PhotonNetwork.NickName = nicknameField.text.Trim();
        
        Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NicknameSuccess]);       // ui modal
        
        PhotonNetwork.JoinLobby();
        
    }

    #endregion

    #region Lobby PunCallbacks

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        _uiNicknamePanel.SetActive(false);          // close window
        lobbyPanel.SetActive(true);
        userLabel.SetActive(true);                  // activate user label canvas
        
        //shortcut 활성화
        UI_Shortcut shortcutUI = Manager.UI.GetGlobalUI(GlobalUI.UI_Shortcut) as UI_Shortcut;
        if (shortcutUI.gameObject.activeSelf) return;
        shortcutUI.InitSetting();
        shortcutUI.Show();
    }

    // 방 리스트 갱신
    public override void OnRoomListUpdate(List<RoomInfo> updateList)
    {
        foreach (RoomInfo info in updateList)
        {
            if (info.RemovedFromList)
            {
                // 삭제된 방 제거
                if (roomList.TryGetValue(info.Name, out GameObject obj))
                {
                    Destroy(obj);
                    roomList.Remove(info.Name);
                }
                continue;
            }

            if (roomList.ContainsKey(info.Name))
            {
                // 방 정보 업데이트
                roomList[info.Name].GetComponent<RoomList>().Init(info, _uiLobby);
            }
            else
            {
                GameObject roomListItem = Instantiate(roomListPrefabs);
                roomListItem.transform.SetParent(roomListContent, false);
                roomListItem.GetComponent<RoomList>().Init(info, _uiLobby);
                roomList.Add(info.Name, roomListItem);
            }
        }
    }
    #endregion

  
    #region Create Room Logic

    // 방 생성
    public void CreateRoom()
    {
        string userRoomName = roomNameField.text;
        if (string.IsNullOrEmpty(userRoomName))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.CreateRoomError]);
            return;
        }
        
        roomNameAdmitButton.interactable = false;
        
        // 1. 고유 코드 생성 (중복 검사 포함)
        var (roomCode, roomName) = GenerateRoomCode();
        if (string.IsNullOrEmpty(roomCode) || string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("방 코드 생성 실패. 방 생성 중단.");
            return;
        }
        
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable
        {
            { "roomCode", roomCode },
            { "userRoomName", userRoomName }
        };
        
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 2,
            CustomRoomProperties = customProperties,
            CustomRoomPropertiesForLobby = new[] { "roomCode", "userRoomName" }
        };
        
        PhotonNetwork.CreateRoom(roomName, options);
        
        Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.CreeateRoomSuccess]);
        
        //패널 닫기
        _uiLobby.CreateRoomPanel.CloseWindow();
        
        roomNameField.text = null;
    }

    //GUID 앞부분만 잘라서 고유 코드로 생성
    private (string roomCode, string guidRoomName) GenerateRoomCode(int length = 6, int retryCount = 0)
    {
        
        if (retryCount >= 5)
        {
            Debug.LogError("방 코드 중복으로 인해 방 생성 실패");
            return (null, null);
        }
        
        // 1. GUID로 고유한 방 이름 생성
        string guid = Guid.NewGuid().ToString();
        string roomName = guid.Replace("-", "").ToUpper();
        
        // 2. GUID 일부를 6자리 코드로 변환 (중복 방지를 위해 소문자 제거)
        string roomCode = roomName.Substring(0,length);
        
        // 3. 중복 확인
        if (CheckRoomCodeDuplicate(roomCode))
        {
            Debug.LogWarning($"[GenerateRoomIdentifiers] 중복 코드: {roomCode}, 재시도...");
            GenerateRoomCode(length, retryCount + 1);
        }

        return (roomCode, roomName); // roomName = guid

    }
    
    private bool CheckRoomCodeDuplicate(string code)
    {
        return roomList.Values.Any(go =>
        {
            RoomList room = go.GetComponent<RoomList>();
            return room.RoomCode == code;
        });

    }

    #endregion

    #region Join Room By Code

    public void JoinRoomByCode()
    {
        string roomCode = codeInputField.text.Trim().ToUpper();
        if (string.IsNullOrEmpty(roomCode))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeError]);
            return;
        }

        string matchedRoomName = roomList.Values
            .Select(go => go.GetComponent<RoomList>())
            .FirstOrDefault(room => room.RoomCode == roomCode)
            ?.RoomName;
        
        if (string.IsNullOrEmpty(matchedRoomName))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeError]);
            return;
        }
        
        if (PhotonNetwork.InLobby)
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeSuccess]);
            
            PhotonNetwork.JoinRoom(matchedRoomName);
            
            _uiLobby.JoinRoomPanel.CloseWindow();
        
            codeInputField.text = null;
          
        }
        
        
        roomNameAdmitButton.interactable = false;

    }
   

    #endregion

    #region Room PunCallbacks

    // 방 생성 완료
    public override void OnCreatedRoom()
    {
        roomNameAdmitButton.interactable = true; // 버튼 활성화
    }
    
    // 방 입장 완료시 
    public override void OnJoinedRoom()
    {
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);
        roomManager.InitRoom();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            roomManager.SetPlayerPanel(player);
        }
    }
    
    
    // 다른 플레이어가 방에 입장했을때 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
            roomManager.SetPlayerPanel(newPlayer);
    }
    
    
    // 다른 플레이어가 방에서 나갔을때 
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
            roomManager.ResetPlayerPanel(otherPlayer);
    }
    
    

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        roomManager.SwitchMasterClient(newMasterClient);
    }
    
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            roomManager.UpdateReadyUI(targetPlayer);
        }
    }
    
    #endregion
    
}
