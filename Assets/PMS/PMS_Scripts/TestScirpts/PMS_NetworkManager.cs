using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.IO;
using ExitGames.Client.Photon;

/*
 * 테스트용 제거 예정
 */

public class PMS_NetworkManager : MonoBehaviourPunCallbacks
{
    
    private void Awake()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 2 }, null);
        Debug.Log("실행?");
    }

    //방에 들어왔을 때 호출
    public override void OnJoinedRoom()
    {
        if(PhotonNetwork.IsMasterClient)
        {
            ExitGames.Client.Photon.Hashtable hash = new ExitGames.Client.Photon.Hashtable();
            hash["GameStart"] = false;
            StartCoroutine(AutoGameStart(10.0f,hash));
        }

        Debug.Log("입장완료");

        //닉네임 설정
        PhotonNetwork.LocalPlayer.NickName = $"Player_{Random.Range(1, 1000).ToString("0000")}";

        //들어온 유저 플레이어 데이터 생성
        PlayerData newPlayerData = new PlayerData(PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.UserId, 0, 0);

        //게임 플레이어 생성 Manager 소환 - 각자 생성
        GameObject gamePlayerManager = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        gamePlayerManager.GetComponent<InGamePlayerManager>()._playerData = newPlayerData;

        /*if (PhotonNetwork.IsMasterClient)
        {
            //게임 플레이어 생성 Manager 소환 - 마스터클라이언트만
            GameObject gamePlayerManager = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerManager"), Vector3.zero, Quaternion.identity);
        }*/
    }

    //임시 테스트 코드
    //방장이 룸에 들어오면 자동으로 Count값만큼 지난후 게임 시작할 수 있도록 
    //룸 프로퍼티값으로 조정 
    private IEnumerator AutoGameStart(float Count,ExitGames.Client.Photon.Hashtable hash)
    {
        yield return new WaitForSeconds(Count);
        hash["GameStart"] = true;
        PhotonNetwork.CurrentRoom.SetCustomProperties(hash); // 변경된 상태를 모든 클라이언트에 동기화
    }

    // 새로운 플레이어가 방에 입장했을 때 (모든 클라이언트에서 호출)
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {   
        if (!PhotonNetwork.IsMasterClient) return;

        base.OnPlayerEnteredRoom(newPlayer);

        PlayerData newPlayerData = new PlayerData(PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.UserId, 0, 0);

        //플레이어 Data를 마스터 클라이언트가 일단 다들고있자
        PlayerManager.Instance._playerDataList.Add(newPlayerData);

        //여기서는 알고 있는데 각자 생성할 시 해당 객체에 대한 GamePlayer -> _data에 대한 정보가 없음 마스터 클라이언트가 소환하지 않기 때문에 정보가 없다.
        //소환 하면 마스터 클라이언트에게 정보를 넘겨주자
    }

    // 플레이어가 방을 나갔을 때 (모든 클라이언트에서 호출)
    public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
    {
        base.OnPlayerLeftRoom(otherPlayer);
        Debug.Log($"{otherPlayer.NickName} 님이 방을 나갔습니다.");

    }
}
/*
        클라이언트 측에서 서버쪽으로 요청이 필요한 경우, PhotonNetwork 클래스를 사용하여 요청을 처리
        PhotonNetwork.ConnectUsingSettings();   // 접속 시도 요청
        PhotonNetwork.Disconnect();             // 접속 해제 요청

        PhotonNetwork.CreateRoom("RoomName");   // 방 생성 요청
        PhotonNetwork.JoinRoom("RoomName");     // 방 입장 요청
        PhotonNetwork.LeaveRoom();              // 방 퇴장 요청

        PhotonNetwork.JoinLobby();              // 로비 입장 요청
        PhotonNetwork.LeaveLobby();             // 로비 퇴장 요청

        PhotonNetwork.LoadLevel("SceneName");   // 씬 전환 요청

        bool isConnected = PhotonNetwork.IsConnected;           // 접속 여부 확인
        bool isInRoom = PhotonNetwork.InRoom;                   // 방 입장 여부 확인
        bool isLobby = PhotonNetwork.InLobby;                   // 로비 입장 여부 확인
        ClientState state = PhotonNetwork.NetworkClientState;   // 클라이언트 상태 확인
        Player player = PhotonNetwork.LocalPlayer;              // 포톤 플레이어 정보 확인
        Room players = PhotonNetwork.CurrentRoom;               // 현재 방 정보 확인


        ------------------------------------------------------------------------------------------------------------------


        public class NetworkManager : MonoBehaviourPunCallbacks
        {
            public override void OnConnected() { }                          // 포톤 접속시 호출됨
            public override void OnConnectedToMaster() { }                  // 마스터 서버 접속시 호출됨
            public override void OnDisconnected(DisconnectCause cause) { }  // 접속 해제시 호출됨

            public override void OnCreatedRoom() { }    // 방 생성시 호출됨
            public override void OnJoinedRoom() { }     // 방 입장시 호출됨
            public override void OnLeftRoom() { }       // 방 퇴장시 호출됨
            public override void OnPlayerEnteredRoom(Player newPlayer) { }  // 새로운 플레이어가 방 입장시 호출됨
            public override void OnPlayerLeftRoom(Player otherPlayer) { }   // 다른 플레이어가 방 퇴장시 호출됨
            public override void OnCreateRoomFailed(short returnCode, string message) { }   // 방 생성 실패시 호출됨
            public override void OnJoinRoomFailed(short returnCode, string message) { }     // 방 입장 실패시 호출됨

            public override void OnJoinedLobby() { }    // 로비 입장시 호출됨
            public override void OnLeftLobby() { }      // 로비 퇴장시 호출됨
            public override void OnRoomListUpdate(List<RoomInfo> roomList) { }  // 방 목록 변경시 호출됨
        }


        ------------------------------------------------------------------------------------------------------------------


        Room room = PhotonNetwork.CurrentRoom;  // 현재 참가한 룸을 확인

        // 룸 커스텀 프로퍼티 설정
        ExitGames.Client.Photon.Hashtable roomProperty = new ExitGames.Client.Photon.Hashtabl> ();
        roomProperty["Map"] = "Select Map";
        room.SetCustomProperties(roomProperty);

        // 룸 커스텀 프로퍼티 확인
        string curMap = (string)room.CustomProperties["Map"];

        Player player = PhotonNetwork.LocalPlayer;  // 자신 플레이어를 확인

        // 플레이어 커스텀 프로퍼티 설정
        ExitGames.Client.Photon.Hashtable playerProperty = new ExitGames.Client.Photon> Hashtable();
        playerProperty["Ready"] = true;
        player.SetCustomProperties(playerProperty);

        // 플레이어 커스텀 프로퍼티 확인
        bool ready = (bool)player.CustomProperties["Ready"];
        아래의 콜백함수를 통해서 방의 정보와 플레이어의 정보가 변경되는 상황을 확인할 수 있습니다.
        public class NetworkManager : MonoBehaviourPunCallbacks
        {
            public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
            {
                // 현재 참여한 방의 프로퍼티가 업데이트시 호출됨
            }

            public override void OnPlayerPropertiesUpdate(Player targetPlayer, ExitGames.Client.Photon.Hashtable changedProps)
            {
                // 같은 방의 플레이어의 프로퍼티가 업데이트시 호출됨
            }
        }
 */
