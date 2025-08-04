using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers;

public class SceneInit : MonoBehaviour
{
    [SerializeField] private string playerPrefabName;

    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "LTH_GameScene") return;

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            StartCoroutine(InitFlow());
        }
    }

    private IEnumerator InitFlow()
    {
        // 플레이어 스폰
        yield return SpawnPlayerWithDelay();

        // 마스터만 게임 시작 진행
        if (PhotonNetwork.IsMasterClient)
        {
            yield return WaitForBoxSpawner();

            // 게임 시작
            InGameManager.Instance.StartGame();
        }
    }

    private IEnumerator SpawnPlayerWithDelay()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;

        yield return new WaitUntil(() => GameObject.Find("GameTable") != null);

        Transform tableCenter = GameObject.Find("GameTable").transform;

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints.Length > actorIndex ? spawnPoints[actorIndex].position : Vector3.zero;

        var obj = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);

        Vector3 dir = (tableCenter.position - obj.transform.position).normalized;
        dir.y = 0f;
        obj.transform.rotation = Quaternion.LookRotation(dir);

        GamePlayer gp = obj.GetComponent<GamePlayer>();
        if (gp != null && gp.GetComponent<PhotonView>().IsMine)
        {
            string nickname = PhotonNetwork.NickName;
            string playerId = PhotonNetwork.LocalPlayer.UserId;

            gp._data = new PlayerData(nickname, playerId, 0, 0);
            gp.SendMyPlayerDataRPC();

            // 카메라 회전 보정
            Camera cam = obj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.rotation = Quaternion.LookRotation((tableCenter.position - cam.transform.position).normalized);
            }
        }
    }

    private IEnumerator WaitForBoxSpawner()
    {
        float timer = 0f;
        float timeout = 2f;

        while (!ItemBoxSpawnerManager.Instance?.IsInitialized ?? true)
        {
            if (timer > timeout)
            {
                Debug.LogWarning("[SceneInit] 아이템 박스 스포너 초기화 대기 타임아웃!");
                break;
            }

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
    }
}