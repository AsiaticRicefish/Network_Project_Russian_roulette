using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestPlayerSpawn : MonoBehaviourPun
{
    [SerializeField] private GameObject playerPrefab;

    private Vector3[] spawnPositions = new Vector3[]
    {
        new Vector3(0f, 0.5f, -2.85f), 
        new Vector3(0f, 0.5f,  2.85f)
    };
    private Quaternion[] spawnRotations = new Quaternion[]
   {
        Quaternion.identity,  
        Quaternion.Euler(0, 180f, 0)    
   };
    private void Start()
    {
        if (PhotonNetwork.IsConnected && PhotonNetwork.InRoom)
        {
            int index = Mathf.Clamp(PhotonNetwork.LocalPlayer.ActorNumber - 1, 0, spawnPositions.Length - 1);
            Vector3 spawnPos = spawnPositions[index];
            Quaternion spawnRot = spawnRotations[index];

            PhotonNetwork.Instantiate("TestPlayer", spawnPos, spawnRot);
        }
    }
}
