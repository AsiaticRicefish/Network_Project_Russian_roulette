using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneInit : MonoBehaviour
{
    [SerializeField] private string playerPrefabName = "PlayerTest";

    [SerializeField] private Transform[] spawnPoints;

    private void Start()
    {
        StartCoroutine(SpawnPlayerWithDelay());
    }

    private IEnumerator SpawnPlayerWithDelay()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
        {
            Debug.Log("[SceneInit] Photon 연결 대기 중...");
            yield return null;
        }

        Debug.Log($"[SceneInit] 연결 완료 → IsMaster: {PhotonNetwork.IsMasterClient}");

        int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1;
        Vector3 spawnPos = spawnPoints.Length > actorIndex ? spawnPoints[actorIndex].position : Vector3.zero;

        var obj = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);
        Debug.Log($"[SceneInit] Instantiate 완료: {obj.name}");
    }
}
