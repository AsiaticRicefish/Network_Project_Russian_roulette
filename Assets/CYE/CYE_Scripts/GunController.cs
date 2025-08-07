using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using System;

public class GunController : MonoBehaviourPun
{
    private Animator _animator;

    public bool _isHold = false;
    // public event Action<bool> OnHolded;

    private PhotonView _photonview;

    [SerializeField] private GameObject _targetSelectUI;

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _photonview = GetComponent<PhotonView>();
    }
    void OnMouseDown()
    {
        if (TurnSync.CurrentTurnPlayerId == PhotonNetwork.LocalPlayer.NickName && !_isHold)
        {
            _isHold = true;
            Debug.Log($"[GunController] {photonView}");
            photonView.RPC(nameof(GunController.SyncHold), RpcTarget.All, true);
            Debug.Log($"[GunController] {_isHold}");
            // OnHolded?.Invoke(_isHold);

            _targetSelectUI.SetActive(true);
        }
    }
    [PunRPC]
    public void SyncHold(bool isHold)
    {
        _isHold = isHold;
        Debug.Log($"[GunController] {_isHold}");
    }
}
