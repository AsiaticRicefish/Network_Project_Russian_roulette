using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Photon.Pun;
using Photon.Realtime;
using DesignPattern;

public class NetworkManager : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject nicknamePanel;
    [SerializeField] private TMP_InputField nicknameField;
    [SerializeField] private Button nicknameAdmitButton;
    [SerializeField] private GameObject lobbyPanel;
    [SerializeField] private GameObject roomPanel;
    [SerializeField] private TMP_InputField roomNameField;
    [SerializeField] private Button roomNameAdmitButton;
    [SerializeField] private GameObject roomListPrefabs;
    [SerializeField] private Transform roomListContent;
    [SerializeField] private RoomManager roomManager;
    private Dictionary<string, GameObject> roomList = new Dictionary<string, GameObject>();

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();   // 마스터 서버 접속
        nicknameAdmitButton.onClick.AddListener(NicknameAdmit);
        roomNameAdmitButton.onClick.AddListener(CreateRoom);
    }

    // 마스터 서버 연결
    public override void OnConnectedToMaster()
    {
        Debug.Log("서버 접속 완료");
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
            return;
        }
        PhotonNetwork.NickName = nicknameField.text;
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        nicknamePanel.SetActive(false);
        lobbyPanel.SetActive(true);
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
        roomManager.PlayerPanelSpawn(PhotonNetwork.LocalPlayer);
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
                roomListItem.transform.SetParent(roomListContent);
                roomListItem.GetComponent<RoomList>().Init(info);
                roomList.Add(info.Name, roomListItem);
            }
        }
    }
    // 내가 방에서 나갔을때
    public override void OnLeftRoom()
    {
        roomPanel.SetActive(false);
        lobbyPanel.SetActive(true);
    }
    // 다른 플레이어가 방에서 나갔을때 
    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        if (otherPlayer != PhotonNetwork.LocalPlayer)
            roomManager.PlayerPanelDestroy(otherPlayer);
    }
}
