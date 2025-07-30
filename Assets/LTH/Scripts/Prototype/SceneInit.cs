using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : MonoBehaviour
{
    [SerializeField] private string playerPrefabName = "PlayerTest";

    [SerializeField] private Transform[] spawnPoints;

    [SerializeField] private Transform tableCenter; // 탁자 중심 Transform

    private void Start()
    {
        if (SceneManager.GetActiveScene().name != "LTH_GameScene") return;

        if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        {
            StartCoroutine(SpawnPlayerWithDelay());
        }
    }

    private IEnumerator SpawnPlayerWithDelay()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;

        // 테이블이 완전히 생성될 때까지 대기
        yield return new WaitUntil(() => GameObject.Find("GameTable") != null);

        Transform tableCenter = GameObject.Find("GameTable").transform;

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints.Length > actorIndex ? spawnPoints[actorIndex].position : Vector3.zero;

        var obj = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
        Debug.Log($"[SceneInit] 플레이어 소환: {obj.name}");

        // 회전 적용
        Vector3 dir = (tableCenter.position - obj.transform.position).normalized;
        dir.y = 0f;
        obj.transform.rotation = Quaternion.LookRotation(dir);
        Debug.Log($"[SceneInit] {obj.name}이 테이블 방향으로 회전됨");

        // 카메라 회전 보정
        Camera cam = obj.GetComponentInChildren<Camera>();
        if (cam != null)
        {
            cam.transform.rotation = Quaternion.LookRotation((tableCenter.position - cam.transform.position).normalized);
        }
    }
}
