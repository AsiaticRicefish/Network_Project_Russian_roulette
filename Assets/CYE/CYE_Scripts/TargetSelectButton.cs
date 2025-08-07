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
    private FireSync _fireSync;
    private GunController _gunController;

    private void Awake()
    {
        _photonView = GetComponentInParent<PhotonView>();
        _fireSync = GetComponentInParent<TargetSelectUI>()._fireSync;
        _gunController = GetComponentInParent<TargetSelectUI>()._gunController;
    }

    public void SetTargetId(string targetId, string targetNickname)
    {
        _targetId = targetId;
        transform.Find("ButtonText").GetComponent<TMP_Text>().text = Utils.Util_LDH.GetUserNickname(targetNickname);
    }
    public void FireToTarget()
    {
        Debug.Log($"[GunController] {_targetId}");
        transform.parent.gameObject.SetActive(false);
        // Managers.Manager.Gun.Fire(_targetId);
        _fireSync.photonView.RPC(nameof(FireSync.RequestFire), RpcTarget.All, PhotonNetwork.LocalPlayer.NickName, _targetId);
        //_gunController.photonView.RPC(nameof(GunController.SyncHold), RpcTarget.All, false);
    }
}
