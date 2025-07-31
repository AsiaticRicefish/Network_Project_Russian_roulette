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

/// <summary>
/// Photon 네트워크 연결 및 유저 UI 흐름 전체를 제어하는 매니저
/// </summary>
public class NetworkManager : MonoBehaviourPunCallbacks
{
    #region UI 연결 필드
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

    #endregion
    
    #region UI 스크립트 참조 캐싱
    private UI_Nickname _uiNicknamePanel;
    private UI_Lobby _uiLobby;
    private ModalWindowManager _createRoomManager;
    #endregion

    /// <summary>
    /// 로비에서 보여지는 방 리스트 캐싱 (Key: roomName)
    /// </summary>
    private Dictionary<string, GameObject> roomList = new Dictionary<string, GameObject>();
    public IReadOnlyList<GameObject> RoomList => roomList.Values.ToList();

    
    private void Awake() => Init();
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   // 마스터 서버 접속
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);
        roomNameAdmitButton.onClick.AddListener(CreateRoom);
        codeJoinButton.onClick.AddListener(JoinRoomByCode);
    }

    #region Init 초기화
    
    private void Init()
    {
        PhotonNetwork.AutomaticallySyncScene = true; 
        
        // 연결된 UI 스크립트 참조
        _uiNicknamePanel = nicknamePanel.GetComponent<UI_Nickname>();
        _uiLobby = lobbyPanel.GetComponent<UI_Lobby>();
        
        InitUIVisibiity();
    }

    /// <summary>
    /// 모든 UI 비활성화 및 초기 상태 세팅
    /// </summary>
    private void InitUIVisibiity()
    {
        loadingPanel.SetActive(true);               // 로딩 ON
        lobbyPanel.SetActive(false);                // 로비 UI OFF
        userLabel.SetActive(false);                 // 유저 라벨 OFF
        _uiNicknamePanel.SetActive(false);          // 닉네임 창 닫기
        _uiLobby.CreateRoomPanel.CloseWindow();     // 방 생성 패널 닫기
        _uiLobby.JoinRoomPanel.CloseWindow();       // 코드 입력 패널 닫기
        roomPanel.SetActive(false);                 // 방 UI OFF
    }
    
    #endregion
    
    #region Connect PunCallbacks
    /// <summary>
    /// 마스터 서버 연결 완료 시 처리
    /// - 첫 접속이면 닉네임 입력창 활성화
    /// - 재접속이면 자동으로 로비 진입
    /// </summary>
    public override void OnConnectedToMaster()
    {
        if (loadingPanel.activeSelf)
        {
            loadingPanel.SetActive(false);
            _uiNicknamePanel.SetActive(true);     // 닉네임 panel 활성화(open window)
        }
        else
        {
            PhotonNetwork.JoinLobby();            // 로비로 진입
            roomPanel.SetActive(false);           // 룸 panel 비활성화
        }
    }
    
    /// <summary>
    /// 서버 연결 끊어졌을 때 재접속 시도
    /// </summary>
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.ConnectUsingSettings(); // 재접속
    }
    

    #endregion
    
    #region NickName

    /// <summary>
    /// 유저 닉네임 설정 후 로비 진입 시도
    /// </summary>
    public void NicknameAdmit()
    {
        // 닉네임 이미 설정된 상태
        if (!string.IsNullOrEmpty(PhotonNetwork.NickName))
            return;
        
        string nickname = nicknameField.text.Trim();
        if (string.IsNullOrWhiteSpace(nickname))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NicknameError]);     // ui modal
            return;
        }
        
        PhotonNetwork.NickName = nickname;
        Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NicknameSuccess]);       // ui modal
        
        PhotonNetwork.JoinLobby();     // 로비 진입
        
    }

    #endregion
    
    #region Lobby PunCallbacks (로비 연결 콜백)

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        
        _uiNicknamePanel.SetActive(false);    // 닉네임 설정창 닫기
        lobbyPanel.SetActive(true);           // 로비 UI 활성화
        userLabel.SetActive(true);            // 유저 정보 라벨 활성화
        
        // 글로벌 단축키 UI 세팅
        UI_Shortcut shortcutUI = Manager.UI.GetGlobalUI(GlobalUI.UI_Shortcut) as UI_Shortcut;
        if (!shortcutUI.gameObject.activeSelf)
        {
            shortcutUI.InitSetting();
            shortcutUI.Show();
        }
    }

    /// <summary>
    /// 로비 내 방 리스트 갱신 콜백
    /// </summary>
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
                // 방 정보 업데이트 (기존 방 정보 갱신)
                roomList[info.Name].GetComponent<RoomList>().Init(info, _uiLobby);
            }
            else
            {
                // 새로운 방 추가
                GameObject roomListItem = Instantiate(roomListPrefabs);
                roomListItem.transform.SetParent(roomListContent, false);
                roomListItem.GetComponent<RoomList>().Init(info, _uiLobby);
                roomList.Add(info.Name, roomListItem);
            }
        }
    }
    #endregion
    
    #region Create Room Logic (방 생성 처리)

    
    /// <summary>
    /// UI에서 방 생성 요청 시 호출
    /// </summary>
    public void CreateRoom()
    {
        string userRoomName = roomNameField.text;
        if (string.IsNullOrEmpty(userRoomName))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.CreateRoomError]);
            return;
        }
        
        roomNameAdmitButton.interactable = false;
        
        // 1. 고유 코드 생성 (GUID 기반 고유 roomName, roomCode 생성)
        var (roomCode, roomName) = GenerateRoomCode();
        if (string.IsNullOrEmpty(roomCode) || string.IsNullOrEmpty(roomName))
        {
            Debug.LogError("[Create Room] 방 코드 생성 실패. 방 생성 중단.");
            return;
        }
        
        // 2. 방에 포함될 커스텀 속성 정의
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
        
        // 3. 방 생성 및 속성 적용
        PhotonNetwork.CreateRoom(roomName, options);
        
        // 4. 모달 알림 팝업
        Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.CreeateRoomSuccess]);
        
        // 5. panel 닫기 (close window)
        _uiLobby.CreateRoomPanel.CloseWindow();
        
        // 6. inputfield 초기화
        roomNameField.text = null;
    }

    /// <summary>
    /// 고유한 방 코드와 GUID RoomName 생성
    /// </summary>
    private (string roomCode, string guidRoomName) GenerateRoomCode(int length = 6, int retryCount = 0)
    {
        
        if (retryCount >= 5)
        {
            Debug.LogError("[GenerateRoomCode] 방 코드 중복으로 인해 방 생성 실패");
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
            Debug.LogWarning($"[GenerateRoomCode] 중복 코드: {roomCode}, 재시도...");
            GenerateRoomCode(length, retryCount + 1);       // 재귀 호출
        }

        return (roomCode, roomName); // roomName = guid

    }
    
    /// <summary>
    /// 현재 방 리스트에 중복 roomCode가 있는지 확인
    /// </summary>
    private bool CheckRoomCodeDuplicate(string code)
    {
        return roomList.Values.Any(go =>
        {
            RoomList room = go.GetComponent<RoomList>();
            return room.RoomCode == code;
        });

    }

    #endregion

    #region Join Room By Code (방 코드로 방 입장)

    /// <summary>
    /// 코드 입력을 통해 특정 방에 입장 시도
    /// </summary>
    public void JoinRoomByCode()
    {
        string roomCode = codeInputField.text.Trim().ToUpper();
        
        //빈 문자열 입력
        if (string.IsNullOrEmpty(roomCode))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeEmpty]);
            return;
        }

        // 1. roomCode 기준으로 매칭되는 RoomList 객체 탐색
        RoomList matchedRoom = roomList.Values
            .Select(go => go.GetComponent<RoomList>())
            .FirstOrDefault(room => room.RoomCode == roomCode);
        
        // 2. 일치하는 방이 없는 경우
        if (matchedRoom == null)
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeError]);
            return;
        }
        
        // 3. 일치하는 방이 있는 경우 -> 플레이어 수 초과 체크
        if (matchedRoom.RoomInfo.PlayerCount >= matchedRoom.RoomInfo.MaxPlayers)
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.JoinRoomError]);
            return;
        }
        
        // 4. 모든 조건 만족하는 경우 진입처리
        if (PhotonNetwork.InLobby)
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.RoomCodeSuccess]);       // 모달 팝업
            
            PhotonNetwork.JoinRoom(matchedRoom.RoomName);               // 방 입장
            
            _uiLobby.JoinRoomPanel.CloseWindow();                       // 패널 비활성화
            
            codeInputField.text = null;                                 // inputfield 초기화
          
        }
        codeJoinButton.interactable = false;
    }
    
    #endregion

    #region Room PunCallbacks

    /// <summary>
    /// 방 생성 성공 시 호출됨
    /// </summary>
    public override void OnCreatedRoom()
    {
        roomNameAdmitButton.interactable = true;    // 버튼 복원 (방 생성 시 비활성화 시킨 버튼 복원)
    }
    
    
    // /// <summary>
    // /// 방 입장 성공 시 UI 상태 초기화 및 플레이어 패널 생성
    // /// </summary>
    public override void OnJoinedRoom()
    {
        
        lobbyPanel.SetActive(false);
        roomPanel.SetActive(true);

        codeJoinButton.interactable = true;         // 버튼 복원 (코드로 방 입장 시 비활성화 시켰던 버튼 복원)
        
        roomManager.InitRoom();                     // UI 및 설정 초기화
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            roomManager.SetPlayerPanel(player);     // 각 플레이어 UI 설정
        }
    }
    
    
    /// <summary>
    /// 다른 플레이어가 방에 새로 입장했을 때 호출
    /// </summary>
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
            roomManager.SetPlayerPanel(newPlayer);      // 들어온 플레이어의 UI 설정
    }
    
    
    /// <summary>
    /// 다른 플레이어가 방에서 나갔을 때 호출
    /// </summary>
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
            roomManager.ResetPlayerPanel(otherPlayer);  // 나간 플레이어의 UI 초기화
    }
    
    
    /// <summary>
    /// 방장이 바뀌었을 때 호출됨 (기존 HOST 퇴장)
    /// </summary>
    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal) // 바뀐 마스터 플레이어가 로컬인 쪽에서만 UI 갱신 및 설정 초기화
        {
            roomManager.SwitchMasterClient(newMasterClient);
        }
     
    }
    
    /// <summary>
    /// 플레이어의 Custom Property (Ready 등) 변경 시 호출됨
    /// </summary>
    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        if (changedProps.ContainsKey("Ready"))
        {
            roomManager.UpdateReadyUI(targetPlayer);
        }
    }
    
    #endregion
    
}
