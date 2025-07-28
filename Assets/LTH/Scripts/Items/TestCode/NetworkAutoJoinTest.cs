using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkAutoJoinTest : MonoBehaviourPunCallbacks
{
    [SerializeField] private string testSceneName = "LTH_GameScene";
    [SerializeField] private int requiredPlayerCount = 2;

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("DevTestRoom", new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);
    }

    public override void OnJoinedRoom()
    {
        Debug.Log($"[Launcher] 현재 인원: {PhotonNetwork.CurrentRoom.PlayerCount}");

        if (PhotonNetwork.IsMasterClient)
            TryStartTest();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Debug.Log($"[Launcher] 새 플레이어 입장: {newPlayer.NickName}");

        if (PhotonNetwork.IsMasterClient)
            TryStartTest();
    }

    private void TryStartTest()
    {
        int currentCount = PhotonNetwork.CurrentRoom.PlayerCount;
        if (currentCount >= requiredPlayerCount)
        {
            Debug.Log($"[Launcher] {currentCount}명 입장 완료 → 테스트 씬 진입");
            PhotonNetwork.AutomaticallySyncScene = true;
            PhotonNetwork.LoadLevel(testSceneName);
        }
        else
        {
            Debug.Log($"[Launcher] 대기 중: 현재 {currentCount}/{requiredPlayerCount}");
        }
    }
}