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

namespace PMS_Test
{
    public class PMS_NetworkManager : MonoBehaviourPunCallbacks
    {
        private void Awake()
        {
            PhotonNetwork.ConnectUsingSettings();
        }

        public override void OnConnectedToMaster() => PhotonNetwork.JoinOrCreateRoom("Room", new RoomOptions { MaxPlayers = 4 }, null);

        //방에 들어왔을 때 호출
        public override void OnJoinedRoom()
        {
            Debug.Log("입장완료");
            PhotonNetwork.LocalPlayer.NickName = $"Player_{Random.Range(1, 1000).ToString("0000")}";

            //SpawnManager.Instance.InitializeAvailableSpawnPoints();
            //InstantiateLocalPlayerCharacter();

            //마스터 클라이언트만 스폰 위치 초기화 실행
            if (PhotonNetwork.IsMasterClient)
            {
                SpawnManager.Instance.InitializeAvailableSpawnPoints();
                //StartCoroutine(SpawnAllPlayersWhenReady());
            }
            /*if(PhotonNetwork.IsMasterClient)
            {
                SpawnManager.Instance.InitializeAvailableSpawnPoints();
                StartCoroutine(HeyWait());
            }
            else
            {
                StartCoroutine(HeyWait());
            }*/
        }

        private IEnumerator HeyWait()
        {
            yield return new WaitForSeconds(0.5f);
            InstantiateLocalPlayerCharacter();
        }

        private void InstantiateLocalPlayerCharacter()
        {
            // 스폰 지점 할당은 오직 마스터 클라이언트만 수행
            // 다른 클라이언트들은 마스터 클라이언트가 스폰한 캐릭터를 감지하고, OnRoomPropertiesUpdate 콜백을 통해 스폰 지점 상태 변화를 인지함.
            if (PhotonNetwork.IsMasterClient)
            {
                Debug.Log("InstantiateLocalPlayerCharacter: 마스터 클라이언트입니다. 스폰 지점 할당 시도 전 룸 프로퍼티 상태:");
                if (PhotonNetwork.CurrentRoom != null && PhotonNetwork.CurrentRoom.CustomProperties != null)
                {
                    foreach (DictionaryEntry prop in PhotonNetwork.CurrentRoom.CustomProperties)
                    {
                        Debug.Log($"  Key: {prop.Key}, Value: {prop.Value}");
                    }
                }
                else
                {
                    Debug.LogWarning("InstantiateLocalPlayerCharacter: Photon Room 또는 CustomProperties가 아직 준비되지 않았습니다.");
                }

                // SpawnManager에서 스폰 지점과 그 인덱스를 함께 가져옵니다.
                (Transform spawnPoint, int spawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

                if (spawnPoint != null && spawnIndex != -1) // 스폰 지점을 성공적으로 얻었다면
                {
                    // PhotonNetwork.Instantiate를 사용하여 플레이어 캐릭터/컨트롤러 프리팹을 네트워크 상에 생성
                    // 프리팹 경로 및 이름 확인 (예: Assets/Resources/Prefabs/PlayerController)
                    GameObject playerobj = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), spawnPoint.position, spawnPoint.rotation);            // 이 부분이 바로 진행 할 필요가 없을 것 같다. JoinRoom했을 때 에는 단순히 PlayerData까지만 들고 있어도 되지 않을까

                    // GamePlayer 컴포넌트를 가져와 초기화 RPC 호출
                    GamePlayer newPlayer = playerobj.GetComponent<GamePlayer>();
                    if (newPlayer != null)
                    {
                        // PlayerData 초기화 (Firebase에서 불러온 데이터를 RPC로 전달하는 부분)
                        PlayerData dummyData = new PlayerData(PhotonNetwork.LocalPlayer.NickName, PhotonNetwork.LocalPlayer.UserId,0,0);          //Firebase에서 uid,wincount,losecount를 들고와야함

                        // GamePlayer의 RPC_InitializePlayer 메서드를 모든 클라이언트에서 호출하여 플레이어 데이터 및 스폰 인덱스 동기화
                        newPlayer.GetComponent<PhotonView>().RPC("RPC_InitializePlayer", RpcTarget.AllBuffered,
                            dummyData.nickname, dummyData.playerId, dummyData.winCount, dummyData.loseCount, spawnIndex);

                        // 플레이어를 PlayerManager에 추가 (플레이어 관리)
                        PMS_Test.PlayerManager.Instance.RegisterPlayer(newPlayer); // Namespace PMS_Test 가정
                        Debug.Log($"로컬 플레이어 컨트롤러({playerobj.name})를 성공적으로 생성하고 PlayerManager에 추가했습니다!");
                    }
                    else
                    {
                        Debug.LogError("스폰된 PlayerController 프리팹에 GamePlayer 컴포넌트가 없습니다!");
                    }
                }
                else
                {
                    Debug.LogError("플레이어 컨트롤러를 스폰할 수 있는 스폰 지점이 없습니다!");
                }
            }
            // 마스터 클라이언트가 아닌 경우, 마스터 클라이언트가 스폰할 플레이어를 기다립니다.
        }

        // 룸 프로퍼티가 변경될 때 호출되는 콜백 (모든 클라이언트에서 호출)
        public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
        {
            base.OnRoomPropertiesUpdate(propertiesThatChanged);
            Debug.Log("룸 프로퍼티 업데이트 감지!");

            // 변경된 프로퍼티 중 스폰 지점 관련 프로퍼티만 확인
            foreach (DictionaryEntry entry in propertiesThatChanged)
            {
                string key = (string)entry.Key;
                if (key.StartsWith(SpawnManager.SP_KEY_PREFIX)) // 스폰 지점 키 접두사로 시작하는지 확인
                {
                    // 키에서 스폰 지점 인덱스 추출
                    int index = int.Parse(key.Replace(SpawnManager.SP_KEY_PREFIX, ""));
                    bool isAvailable = (bool)entry.Value; // 변경된 스폰 지점의 새로운 상태

                    // 이 콜백은 주로 상태 변경을 인지하고 디버깅 로그를 출력하거나,
                    // 스폰 지점 상태에 따라 UI를 업데이트하는 데 사용될 수 있습니다.
                    // SpawnManager의 메서드들은 이미 룸 프로퍼티를 직접 참조하므로,
                    // 여기서 SpawnManager 내부 상태를 별도로 변경할 필요는 없습니다.
                    Debug.Log($"스폰 지점 {index} 상태 변경: {(isAvailable ? "사용 가능" : "사용 중")}");
                }
            }
        }

        // 새로운 플레이어가 방에 입장했을 때 (모든 클라이언트에서 호출)
        public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
        {
            if (PhotonNetwork.IsMasterClient)
            {
                //SpawnPlayerForClient(newPlayer);
            }
        }

        // 플레이어가 방을 나갔을 때 (모든 클라이언트에서 호출)
        public override void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer)
        {
            base.OnPlayerLeftRoom(otherPlayer);
            Debug.Log($"{otherPlayer.NickName} 님이 방을 나갔습니다.");

            // 마스터 클라이언트만 스폰 지점을 반환하는 책임을 가지게
            if (PhotonNetwork.IsMasterClient)
            {
                // 나간 유저의 정보가 필요하다.
                // 방을 나간 플레이어가 사용하던 스폰 지점을 찾아 반환하는 로직 구현 필요.
                // GamePlayer에서 스폰 인덱스를 저장해두었으므로, PlayerManager에서 해당 플레이어를 찾아 인덱스를 얻습니다.
                /*GamePlayer exitedPlayer = PMS_Test.PlayerManager.Instance.FindPlayerByActorNumber(otherPlayer.ActorNumber); // PlayerManager에 FindPlayerByActorNumber 추가 가정
                if (exitedPlayer != null && exitedPlayer.AssignedSpawnPointIndex != -1)
                {
                    SpawnManager.Instance.ReturnSpawnPoint(exitedPlayer.AssignedSpawnPointIndex);
                    Debug.Log($"마스터 클라이언트: {otherPlayer.NickName} 님이 사용했던 스폰 지점 {exitedPlayer.AssignedSpawnPointIndex}를 반환했습니다.");
                }
                else
                {
                    Debug.LogWarning($"마스터 클라이언트: 나간 플레이어 {otherPlayer.NickName}의 스폰 지점을 찾거나 반환할 수 없습니다.");
                }*/
            }
        }
    }
}
