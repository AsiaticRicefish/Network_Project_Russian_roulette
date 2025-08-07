using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _targetButtonArray = new GameObject[2];
    public FireSync _fireSync;
    public GunController _gunController;
    
    private void OnEnable()
    {
        InitializeNameTag();
    }

    public void InitializeNameTag()
    {
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (p == PhotonNetwork.LocalPlayer)
            {
                _targetButtonArray[1].GetComponent<TargetSelectButton>()?.SetTargetId(p.NickName, p.NickName);
            }
            else
            {
                _targetButtonArray[0].GetComponent<TargetSelectButton>()?.SetTargetId(p.NickName, p.NickName);
            }
        }
    }
}
