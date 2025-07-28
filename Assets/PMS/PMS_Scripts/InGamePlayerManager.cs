using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

    //이친구를 생성을 하고 
public class InGamePlayerManager : MonoBehaviourPunCallbacks
{
    private PhotonView _pv;
   
    private string key = "GameStart";
    private bool flag = false;

    public PlayerData _playerData;
    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    public override void OnRoomPropertiesUpdate(ExitGames.Client.Photon.Hashtable propertiesThatChanged)
    {
        if (_pv.IsMine && 
            propertiesThatChanged.ContainsKey(key) &&
            (bool)propertiesThatChanged[key] == true)
        {
            CreateController();
        }
    }

    private void CreateController()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(HeyWait());
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Transform playerSpawnPos;
            int playerSpawnIndex;
            (playerSpawnPos, playerSpawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

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
        yield return new WaitForSeconds(5.0f);

        Transform playerSpawnPos;
        int playerSpawnIndex;
        (playerSpawnPos, playerSpawnIndex) = SpawnManager.Instance.GetAndClaimRandomSpawnPoint();

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

    //마스터 클라이언트 스폰 기준
    /*private void CreateController()
    {
        foreach (Photon.Realtime.Player player in PhotonNetwork.PlayerList)
        {
            GameObject go = PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), Vector3.zero, Quaternion.identity);
            GamePlayer gp = go.GetComponent<GamePlayer>();
            PlayerData pd = PlayerManager.Instance.GetFindPlayerDataFromID(PhotonNetwork.LocalPlayer.UserId);
            //firebase아이디와,nickname을 가져오게 하는 함수가 필요할 것 같다.
            if (pd != null)
            {
                gp.GetComponent<GamePlayer>()._data = pd;
            }

            PhotonView photonView = go.GetComponent<PhotonView>();
            if (photonView != null && photonView.Owner != player)
            {
                photonView.TransferOwnership(player); // 생성 후 바로 해당 플레이어에게 소유권 이전
            }

        }
    }*/
}
