using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using System;
using UnityEngine.EventSystems;

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

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InGameManager.Instance.OnTurnEnd += HoldReset;
        }
    }
    void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject()) return;
        
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

    [PunRPC]
    public void SetTargetSelectUI(GameObject targetUI)
    {
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("[Gun Controller] targetSelectUI 동적으로 할당");
        _targetSelectUI = targetUI;
    }

    public void HoldReset()
    {
        photonView.RPC("SyncHold", RpcTarget.All, false);
    }
}
