using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Realtime;


public class CYE_TestScript : MonoBehaviourPunCallbacks
{
    private PhotonView _photonView;
    void Awake()
    {
        _photonView = GetComponent<PhotonView>();

        Debug.Log("[Lobby] Start 호출됨");

        PhotonNetwork.AutomaticallySyncScene = true;
        string nickname = "Tester_" + Random.Range(1000, 9999);
        PhotonNetwork.NickName = nickname;
        PhotonNetwork.ConnectUsingSettings();

        Debug.Log($"[Lobby] 닉네임 설정됨: {PhotonNetwork.NickName}");

        
    }
    public override void OnConnectedToMaster()
    { 
        PhotonNetwork.JoinOrCreateRoom("testRoomName", new RoomOptions { MaxPlayers = 6 }, TypedLobby.Default);
        
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            // _photonView.RPC(nameof(Manager.Game.RPC_StartGame), RpcTarget.All);
            Manager.Game.StartGame();
        }
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Manager.Game.PauseGame(true);
        }
    }
}
