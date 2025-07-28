using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneManager : MonoBehaviourPunCallbacks
{
    void Start()
    {
        // 게임 씬 진입 시 ItemSync 생성
        PhotonNetwork.Instantiate("ItemSync", Vector3.zero, Quaternion.identity);
    }
}
