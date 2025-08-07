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
        
        var targetUI = FindObjectOfType<TargetSelectUI>(false);
        targetUI.SetGunController(this);
        _targetSelectUI = targetUI.gameObject;
    }
    void OnMouseDown()
    {
        if (TurnSync.CurrentTurnPlayerId == PhotonNetwork.LocalPlayer.NickName && !_isHold)
        {
            _isHold = true;
            Debug.Log($"[GunController] {photonView} / {nameof(GunController.SyncHold)}");
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

    [PunRPC]
    public void SetTargetSelectUI(GameObject targetUI)
    {
        if(!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[Gun Controller] targetSelectUI 동적으로 할당");
        _targetSelectUI = targetUI;
    }
}
