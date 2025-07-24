using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;


/*
 * 테스트용 제거 예정
 */

namespace PMS_Test
{
    public class PMS_NetworkManager : MonoBehaviourPunCallbacks
    {
        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);

        public override void OnJoinedRoom()
        {
            Debug.Log("입장완료");
            PhotonNetwork.LocalPlayer.NickName = $"Player_{Random.Range(1, 1000).ToString("0000")}";
            SpawnPlayer();

        }

        private void SpawnPlayer()
        {
            PlayerData playerData = new PlayerData(PhotonNetwork.LocalPlayer.NickName, "abc" + Random.Range(1, 1000).ToString("0000"), 0, 0);
        }
    }
}