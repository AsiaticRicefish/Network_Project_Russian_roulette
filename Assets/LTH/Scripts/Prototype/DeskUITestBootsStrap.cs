using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeskUITestBootsStrap : MonoBehaviour
{
    private IEnumerator Start()
    {
        yield return new WaitForSeconds(1f); // 씬 로딩 및 네트워크 정리 시간 대기

        if (PhotonNetwork.InRoom)
        {
            DeskUIManager.Instance.CreateDeskUIsForPhotonPlayers();
        }
    }
}
