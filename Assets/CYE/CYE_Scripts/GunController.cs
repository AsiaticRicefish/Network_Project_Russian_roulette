using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using System;

public class GunController : MonoBehaviourPun
{
    private Animator _animator;
    private bool _isHold = false;
    public event Action<bool> OnHolded;

    [SerializeField] private GameObject _targetSelectUI;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }

    void OnMouseDown()
    {
        Debug.Log("pick up");
        if (!_isHold)
        {
            photonView.RPC(nameof(SyncHold), RpcTarget.All, true);
            OnHolded?.Invoke(_isHold);
        }
    }
    [PunRPC]
    public void SyncHold(bool isHold)
    {
        _isHold = isHold;
    }
}
