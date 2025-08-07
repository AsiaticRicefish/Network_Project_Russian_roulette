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

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InGameManager.Instance.OnTurnEnd += HoldReset;
        }
    }
    void OnMouseDown()
    {
        if (TurnSync.CurrentTurnPlayerId == PhotonNetwork.LocalPlayer.NickName && !_isHold)
        {
            _isHold = true;
            Debug.Log($"[GunController] {photonView} / {nameof(SyncHold)}");
            photonView.RPC(nameof(SyncHold), RpcTarget.All, true);
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

    public void HoldReset()
    {
        photonView.RPC("SyncHold", RpcTarget.All, false);
    }
}
