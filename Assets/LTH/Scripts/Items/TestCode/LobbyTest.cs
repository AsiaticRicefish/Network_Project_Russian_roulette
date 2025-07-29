using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyTest : MonoBehaviourPunCallbacks
{
    [Header("UI 패널")]
    [SerializeField] private GameObject panelConnecting;
    [SerializeField] private GameObject panelLoading;

    [Header("기타")]
    [SerializeField] private Button startButton;
    [SerializeField] private string roomName = "DevTestRoom";
    [SerializeField] private string gameSceneName = "LTH_GameScene";
    [SerializeField] private int requiredPlayerCount = 2;

    private bool waiting = false;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        PhotonNetwork.ConnectUsingSettings();

        ShowPanel(panelConnecting);
        startButton.interactable = false;
    }

    public override void OnConnectedToMaster()
    {
        Debug.Log("[Lobby] Photon 연결 성공");
        ShowPanel(null);
        startButton.interactable = true;
    }

    public void OnClick_StartGame()
    {
        Debug.Log("[Lobby] GameStart 버튼 클릭 → 방 입장 시도");
        PhotonNetwork.JoinOrCreateRoom(roomName, new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);

        ShowPanel(panelLoading);
        startButton.interactable = false;
        waiting = true;
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[Lobby] 방 입장 완료. 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");
        TryStartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[Lobby] 플레이어 추가 입장: {newPlayer.NickName}");
        if (waiting)
            TryStartGame();
    }

    private void TryStartGame()
    {
        int count = PhotonNetwork.CurrentRoom.PlayerCount;
        if (count >= requiredPlayerCount && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[Lobby] 조건 충족 → 게임 씬 로딩");
            PhotonNetwork.LoadLevel(gameSceneName);
        }
        else
        {
            Debug.Log($"[Lobby] 대기 중: {count}/{requiredPlayerCount}");
        }
    }

    private void ShowPanel(GameObject target)
    {
        panelConnecting?.SetActive(false);
        panelLoading?.SetActive(false);
        target?.SetActive(true);
    }
}
