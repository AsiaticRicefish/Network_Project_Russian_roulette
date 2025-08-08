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
        
        //싱글톤에 등록하기
        GunManager.Instance.gunController = this;
        
    }

    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InGameManager.Instance.OnTurnEnd += HoldReset;
        }

        var targetUI = FindObjectOfType<TargetSelectUI>(true);
        targetUI.SetGunController(this);
        _targetSelectUI = targetUI.gameObject;
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



    #region Gun Bullet / Damage RPC

    [PunRPC]
    public void RPC_SwitchNextBullet()
    {
        Debug.Log("[GunManager] Master에서 현재 로드된 탄환의 타입을 변경합니다.");
        Debug.Log($"[GunManager] 바꾸기 전 탄환 {GunManager.Instance.LoadedBullet.ToString()}");
        
        GunManager.Instance.SetLoadedBullet((GunManager.Instance.LoadedBullet == BulletType.live)? BulletType.blank : BulletType.live);
        Debug.Log($"[GunManager] 다이얼 사용 -> 바뀐 탄환 {GunManager.Instance.LoadedBullet.ToString()}");
        
        // photonView.RPC(nameof(RPC_LoadedBulletSync),RpcTarget.All, GunManager.Instance.LoadedBullet);
    }

    // [PunRPC]
    // public void RPC_LoadedBulletSync(BulletType loadedBulletType)
    // {
    //     Debug.Log("[GunManager] 장전된 탄환을 동기화합니다.");
    //     Debug.Log($"[GunManager] 동기화 이전 탄환 ({(PhotonNetwork.IsMasterClient?"마스터":"클라이언트")}) : {GunManager.Instance.LoadedBullet}");
    //
    //     GunManager.Instance.SetLoadedBullet(loadedBulletType);
    //     
    //     Debug.Log($"[GunManager] 동기화 완료 후 탄환 ({(PhotonNetwork.IsMasterClient?"마스터":"클라이언트")}) : {GunManager.Instance.LoadedBullet}");
    //     
    // }
    
    [PunRPC]
    public void RPC_SetEnhanced(bool value)
    {
        GunManager.Instance.IsEnhanced = value;
        Debug.Log($"[동기화] isEnhanced = {value}");
    }


    #endregion
}
