using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class PMS_GameManager : MonoBehaviour
{
    private bool flag = false;

    void Update()
    {
        //한번만 실행
        if (PhotonNetwork.InRoom &&
            PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey("GameStart") &&
            (bool)PhotonNetwork.CurrentRoom.CustomProperties["GameStart"] == true && !flag)
        { 
            StartCoroutine(MasterClientSpawnPlayersWithDelay());
            flag = true;  
        }
    }

    //개별적으로 플레이어 생성 및 플레이어들이 다른 유저들에게 자기 정보를 보내는 RPC 함수 전송 및 수신 시간 생각해서 만든 코루틴
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f); 
        //PlayerManager.Instance.AllGamePlayerAdd();
    }

    //마스터 클라이언트가 Spawn담당 할 때
    private IEnumerator MasterClientSpawnPlayersWithDelay()
    {
        yield return new WaitForSeconds(0.1f); // 씬 로딩 및 초기화가 완료될 시간을 주기 위한 약간의 지연
        InGamePlayerManager.Instance.CreateController();
    }
}
