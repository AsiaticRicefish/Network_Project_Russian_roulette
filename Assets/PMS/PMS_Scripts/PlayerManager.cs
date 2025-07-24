using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PlayerManager : MonoBehaviour
{
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if(_pv.IsMine)
        {
            CreateController();
        }
    }

    void CreateController()
    {
        //Instantiate player controller
        PhotonNetwork.Instantiate(Path.Combine("Prefabs", "PlayerConrtoller"), Vector3.zero, Quaternion.identity);
    }
}
