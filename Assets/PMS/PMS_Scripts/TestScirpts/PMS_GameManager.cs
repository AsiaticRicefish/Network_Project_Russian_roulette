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

    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(2.0f);
        PlayerManager.Instance.AllGamePlayerAdd();
    }
}
