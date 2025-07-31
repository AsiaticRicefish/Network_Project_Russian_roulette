using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneInit : MonoBehaviour
{
    [SerializeField] private string playerPrefabName;

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

        // 회전 적용
        Vector3 dir = (tableCenter.position - obj.transform.position).normalized;
        dir.y = 0f;
        obj.transform.rotation = Quaternion.LookRotation(dir);

        // GamePlayer 데이터 전달
        GamePlayer gp = obj.GetComponent<GamePlayer>();
        if (gp != null && gp.GetComponent<PhotonView>().IsMine)
        {
            // 여기에 실제 닉네임과 Firebase UID로 교체
            string nickname = PhotonNetwork.NickName;
            string playerId = PhotonNetwork.LocalPlayer.UserId;
            int win = 0;
            int lose = 0;

            gp._data = new PlayerData(nickname, playerId, win, lose);
            gp.SendMyPlayerDataRPC();  // 모든 클라이언트에 브로드캐스트

            // 카메라 회전 보정
            Camera cam = obj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.rotation = Quaternion.LookRotation((tableCenter.position - cam.transform.position).normalized);
            }
        }
    }
}