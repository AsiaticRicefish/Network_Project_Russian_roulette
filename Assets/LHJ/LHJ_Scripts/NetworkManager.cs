using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using DesignPattern;
using GameUI;
using Managers;
using UnityEngine.PlayerLoop;
using UnityEngine.Rendering.VirtualTexturing;
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
    
    
    [Header("Room")]
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private RoomManager roomManager;
    private Dictionary<string, GameObject> roomList = new Dictionary<string, GameObject>();

    
    #region ui scripts reference (private 접근자)
    private UI_Nickname _uiNicknamePanel;

    #endregion

    private void Awake() => Init();
    
    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   // 마스터 서버 접속
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);
        roomNameAdmitButton.onClick.AddListener(CreateRoom);
    }


    private void Init()
    {
        _uiNicknamePanel = nicknamePanel.GetComponent<UI_Nickname>();
        
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
        //join room by code 패널 invisible
        //room 비활성화
    }

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
        }
    }
    public override void OnDisconnected(DisconnectCause cause)
    {
        base.OnDisconnected(cause);
        PhotonNetwork.ConnectUsingSettings(); // 재접속
    }

    // 닉네임 설정 및 로비진입
    public void NicknameAdmit()
    {
        if (string.IsNullOrWhiteSpace(nicknameField.text))
        {
            Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NickNameError]);     // ui modal
            return;
        }
        
        PhotonNetwork.NickName = nicknameField.text.Trim();
        
        Manager.UI.ShowNotifyModal(NotifyMessage.MessageEntities[NotifyMessageType.NickNameSuccess]);       // ui modal
        
        PhotonNetwork.JoinLobby();
        
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        _uiNicknamePanel.SetActive(false);          // close window
        lobbyPanel.SetActive(true);
        userLabel.SetActive(true);                  // activate user label canvas
    }

    // 방 생성
    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomNameField.text))
        {
            return;
        }

        roomNameAdmitButton.interactable = false;
        RoomOptions options = new RoomOptions { MaxPlayers = 2 };
        PhotonNetwork.CreateRoom(roomNameField.text, options);
        roomNameField.text = null;
    }

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
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            roomManager.PlayerPanelSpawn(player);
        }
    }

    // 다른 플레이어가 방에 입장했을때 
    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (newPlayer != PhotonNetwork.LocalPlayer)
            roomManager.PlayerPanelSpawn(newPlayer);
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
                roomList[info.Name].GetComponent<RoomList>().Init(info);
            }
            else
            {
                GameObject roomListItem = Instantiate(roomListPrefabs);
                roomListItem.transform.SetParent(roomListContent, false);
                //roomListItem.GetComponent<RoomList>().Init(info);
                roomList.Add(info.Name, roomListItem);
            }
        }
    }
    // 다른 플레이어가 방에서 나갔을때 
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
            roomManager.PlayerPanelDestroy(otherPlayer);
    }
}
