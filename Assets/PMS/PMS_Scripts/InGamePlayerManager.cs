using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;
using UnityEngine.SceneManagement;

//Player생성 관리 - 싱글톤
public class InGamePlayerManager : MonoBehaviour
{
    public static InGamePlayerManager Instance;
    private PhotonView _pv;
   
    private string key = "GameStart";
    private bool flag = false;

    public PlayerData _playerData;

    //생성시점이 플레이어 룸에 들어왔을 때
    private void Awake()
    {
        // 인스턴스가 없으면 자신을 인스턴스로 지정
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // 씬 전환 시 파괴 방지
        }
        else
        {
            Destroy(gameObject); // 중복된 싱글톤이 생기면 삭제
        }
        _pv = GetComponent<PhotonView>();
    }


    #region 마스터 클라이언트 기준 Spawn - 버그있음
    /*public void MasterClientSpawnAllPlayers()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("MasterClientSpawnAllPlayers는 마스터 클라이언트에서만 호출되어야 함.");
            return;
        }

        Debug.Log("마스터 클라이언트가 모든 플레이어를 생성");

        // 룸에 있는 모든 플레이어를 순회하는데 먼저 들어온 사람이 앞에존재
        foreach (Photon.Realtime.Player photonPlayer in PhotonNetwork.PlayerList)
        {
            // 각 플레이어에 대한 고유한 스폰 포인트를 얻습니다.
            // SpawnManager는 이미 사용된 스폰 포인트를 다시 반환하지 않도록 구현되어야 합니다.
            (Transform playerSpawnPos, int playerSpawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

            if (playerSpawnPos == null)
            {
                Debug.LogError($"플레이어 {photonPlayer.NickName}을 위한 스폰 포인트가 null입니다! 생성할 수 없습니다.");
                continue; // 스폰 포인트가 없으면 해당 플레이어는 건너뜁니다.
            }

            // PlayerController 프리팹을 인스턴스화합니다.
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), playerSpawnPos.position, Quaternion.identity);
            GamePlayer gamePlayer = go.GetComponent<GamePlayer>();

            if (gamePlayer == null)
            {
                Debug.LogError("인스턴스화된 PlayerController에서 GamePlayer 컴포넌트를 찾을 수 없습니다!");
                PhotonNetwork.Destroy(go); // 정리
                continue;
            }

            string uid = photonPlayer.CustomProperties["uid"]?.ToString();
            PlayerData pd = PlayerManager.Instance?.GetFindPlayerDataFromID(uid);

            if (pd != null)
            {
                gamePlayer._data = pd;
            }
            else
            {*/
    // PlayerManager가 없거나 데이터가 없는 경우를 위한 폴백
    /*if (photonPlayer.IsLocal)
    {
        // 로컬 플레이어인 경우, 이 매니저에 저장된 로컬 플레이어 데이터를 사용합니다.
        gamePlayer._data = _playerData;
    }
    else
    {
        // PlayerManager가 데이터를 제공하지 못하면 기본 데이터를 생성합니다.
        gamePlayer._data = new PlayerData(photonPlayer.NickName, uid, 0, 0);
        Debug.LogWarning($"{photonPlayer.NickName}에 대한 PlayerData를 찾을 수 없습니다. 기본 데이터를 사용합니다.");
    }*/
    /*}
    gamePlayer._spawnPointindex = playerSpawnIndex;

    // 생성된 객체의 소유권을 실제 플레이어에게 이전합니다.
    PhotonView photonView = go.GetComponent<PhotonView>();
    if (photonView != null && photonView.Owner != photonPlayer)
    {
        photonView.TransferOwnership(photonPlayer);
        Debug.Log($"{go.name}의 소유권을 {photonPlayer.NickName}에게 이전했습니다.");
    }
    else if (photonView == null)
    {
        Debug.LogError("PlayerController에서 PhotonView를 찾을 수 없습니다!");
    }

    gamePlayer.SendMyPlayerDataRPC();
}
}*/

    #endregion

    //TODO - 리펙토링의 필요 - 다인전 할 때 생성 순서를 정해줘서 생성해야한다.
    //<Before : 마스터 클라이언트 먼저 생성, After : 나머지 사람 생성> 구조 
    public void CreateController()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(HeyWait());
        }

        if (PhotonNetwork.IsMasterClient)
        {
            (Transform playerSpawnPos, int playerSpawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

            if (playerSpawnPos == null)
            {
                Debug.LogError("playerSpawnPos가 null입니다!");
                return;
            }

            GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), playerSpawnPos.position, Quaternion.identity);
            GamePlayer player = go.GetComponent<GamePlayer>();

            player._data = _playerData;
            player._spawnPointindex = playerSpawnIndex;
            //내가 만들었으니깐 다른 애들은 나에 대한 정보를 모름 알려줘야함.
            player.SendMyPlayerDataRPC();
        }
    }

    private IEnumerator HeyWait()
    {
        Debug.Log("잠시멈춤");
        yield return new WaitForSeconds(1.0f);

        (Transform playerSpawnPos, int playerSpawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

        if (playerSpawnPos == null)
        {
            Debug.LogError("playerSpawnPos가 null입니다!");
        }

        GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), playerSpawnPos.position, Quaternion.identity);
        GamePlayer player = go.GetComponent<GamePlayer>();

        player._data = _playerData;
        player._spawnPointindex = playerSpawnIndex;
        //내가 만들었으니깐 다른 애들은 나에 대한 정보를 모름 알려줘야함.
        player.SendMyPlayerDataRPC();
    }
}
