using UnityEngine;
using Photon.Pun;

public class PlayerSpawn
{
    [SerializeField]private static string PLAYER_PREFAB_PATH = "Prefabs/Player";

    //생성 부분만 빼오기
    public static GameObject CreateGamePlayer(PlayerData playerData, Transform spawnPosition, int spawnIndex)
    { 
        //플레이어 데이터가 있는지 확인
        if (playerData == null)
        {
            Debug.LogError("플레이어 데이터가 null상태 입니다.");
            return null;
        }

        //프리팹 생성
        GameObject playerObject = PhotonNetwork.Instantiate(PLAYER_PREFAB_PATH, spawnPosition.position, spawnPosition.localRotation);

        if (playerObject == null)
        {
            Debug.LogError($"프리팹을 생성 실패 생성 Path : {PLAYER_PREFAB_PATH}");
            return null;
        }

        //GamePlayer컴포넌트 가져오기
        GamePlayer gamePlayer = playerObject.GetComponent<GamePlayer>();
        if (gamePlayer == null)
        {
            Debug.LogError("GamePlayer 컴포넌트를 찾을 수 없습니다!");
            PhotonNetwork.Destroy(playerObject);
            return null;
        }

        InitializePlayer(gamePlayer, playerData, spawnIndex);

        gamePlayer.SendMyPlayerDataRPC(); //네트워크 동기화

        return playerObject;
    }

    private static void InitializePlayer(GamePlayer gamePlayer, PlayerData playerData, int spawnIndex)
    {
        gamePlayer._data = playerData;
        gamePlayer._spawnPointindex = spawnIndex;
    }

    public static GameObject CreateGamePlayerWithAutoSpawn(PlayerData playerData)
    {
        // 스폰 위치 자동 할당
        var spawnInfo = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

        if (spawnInfo.spawnPoint == null)
        {
            return null;
        }

        return CreateGamePlayer(playerData, spawnInfo.spawnPoint, spawnInfo.index);
    }


}


