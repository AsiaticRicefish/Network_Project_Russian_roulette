using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectButton : MonoBehaviourPun
{
    public string _targetId;
    private void Awake()
    {

    }

    public void SetTargetId(string targetId, string targetNickname)
    {
        _targetId = targetId;
        transform.Find("ButtonText").GetComponent<TMP_Text>().text = targetNickname;
    }
    public void FireToTarget()
    {
        // Managers.Manager.Gun.Fire(_targetId);
        Debug.Log($"[GunController] {_targetId}");
        photonView.RPC("Fire", RpcTarget.All, _targetId, (int)GunManager.Instance.LoadedBullet);
        transform.parent.gameObject.SetActive(false);
        photonView.RPC("SyncHold", RpcTarget.All, false);
    }
}
