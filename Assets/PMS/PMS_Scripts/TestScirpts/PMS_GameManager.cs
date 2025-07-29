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
            StartCoroutine(Delay());
            flag = true;  
        }
    }


    //개별적으로 플레이어 생성 및 플레이어들이 다른 유저들에게 자기 정보를 보내는 RPC 함수 전송 및 수신 시간 생각해서 만든 코루틴
    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(0.1f); 
        //PlayerManager.Instance.AllGamePlayerAdd();
    }
}
