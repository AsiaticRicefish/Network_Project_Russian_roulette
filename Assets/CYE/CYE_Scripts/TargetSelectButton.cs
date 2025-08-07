using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectButton : MonoBehaviourPun
{
    public string _targetId;
    private PhotonView _photonView;
    private void Awake()
    {
        _photonView = GetComponentInParent<PhotonView>();
    }

    public void SetTargetId(string targetId, string targetNickname)
    {
        _targetId = targetId;
        transform.Find("ButtonText").GetComponent<TMP_Text>().text = targetNickname;
    }
    public void FireToTarget()
    {
        Debug.Log($"[GunController] {_targetId}");
        transform.parent.gameObject.SetActive(false);
        photonView.RPC(nameof(GunController.SyncHold), RpcTarget.All, false);
        // Managers.Manager.Gun.Fire(_targetId);
        photonView.RPC(nameof(FireSync.RequestFire), RpcTarget.All, _targetId);
    }
}
