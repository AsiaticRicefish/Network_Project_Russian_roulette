using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CYE_TestScript : MonoBehaviour
{
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            foreach (var player in PhotonNetwork.PlayerList)
            {
                Debug.Log($"{player.UserId}");
                if(player.UserId != null)
                    Managers.Manager.Gun.Fire(player.UserId);
            }
            
        }
    }
}
