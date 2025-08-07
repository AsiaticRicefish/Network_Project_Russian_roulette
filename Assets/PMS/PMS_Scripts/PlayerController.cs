using Cinemachine;
using GameCamera;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Utils;
using DG.Tweening;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private float _mouseSensitivity;

    [SerializeField] private GameObject _gun;
    [SerializeField] private GameObject _gunPos;
    [SerializeField] private GameObject _destination;

    private Vector3 _oldGunPos;
    private Vector3 _oldGunRotation;

    private bool _isGunAnim = false;
    
    
    private float moveSpeed = 5.0f;
    private float verticalLookRotation;

    private PhotonView _pv;
    private Rigidbody _rb;

    private IEnumerator gunCorutine;

    private FireSync _fireSync;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        _gun = GameObject.FindWithTag("Gun");       //Gun 오브젝트 찾기
        _oldGunPos = _gun.transform.position;
        _oldGunRotation = _gun.transform.rotation.eulerAngles;

        _fireSync = FindObjectOfType<FireSync>();
       
        if (!_pv.IsMine)
        {
            //자기 카메라가 아니면 다 비활성화 처리
            //추후 게임이 종료되도 관전 모드를 만들게 되면 다른 유저의 시야를 봐야 할 수도 있기 때문에
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            Destroy(_rb);
        }
        
        //  
        //  string nickname = PhotonNetwork.NickName;
        //  string playerId = photonView.Owner.UserId;
        //  Debug.Log($"{nickname}의 player id = {playerId}");
        //  
        //  //player manager에 player를 등록한다.
        //  var gamePlayer = GetComponent<GamePlayer>();
        //  gamePlayer._data = new PlayerData(nickname, playerId, 0, 0);
        //  Manager.PlayerManager.RegisterPlayer(gamePlayer);
        //  
        // Debug.Log($"{nickname}를 PlayerManager에 등록했습니다.");
    }


    // private void Update()
    // {
    //     if (!_pv.IsMine) return;
    //
    //     if (Input.GetKeyDown(KeyCode.Space) && gunCorutine == null && IsGunAnim == false)
    //     {
    //         IsGunAnim = true;
    //         GetGun(_gun.transform, _destination.transform);
    //     }
    //     //PlayerLook();
    // }


    public void PlayFire()
    {
        if (gunCorutine == null && _isGunAnim == false)
        {
            _isGunAnim = true;
            GetGun(_gun.transform, _destination.transform);
        }
    }

    private IEnumerator GunAnimation()
    {
        _animator.SetTrigger("Shot");

        _gun.transform.parent = _gunPos.transform;
        _gun.transform.position = transform.position;

        _gun.transform.localPosition = new Vector3(0, 0, 0);
        _gun.transform.localRotation = Quaternion.Euler(0, 0, 0);

        yield return new WaitForSeconds(2.3f);// new WaitUntil(() => !_animator.GetCurrentAnimatorStateInfo(1).IsName("GunPlay"));

        _gun.transform.parent = null;

        _gun.transform.position = _oldGunPos;
        _gun.transform.rotation = Quaternion.LookRotation(_oldGunRotation);//_oldGunRotation;

        gunCorutine = null;
        _isGunAnim = false;
    }

    private void GetGun(Transform target, Transform destination)
    {
        //Manager.Camera.PlayImpulse(1.0f);
        target.rotation = Quaternion.identity;
        Sequence seq = DOTween.Sequence();
        seq.Append(target.DOMove(destination.position, 1.5f));
        seq.Join(target.DORotate(new Vector3(-45, 90, 0), 0.8f));
        seq.OnComplete(() =>
        {
            Debug.Log("시퀀스 완료!");
            gunCorutine = GunAnimation();
            StartCoroutine(GunAnimation());
        });
    }

    private void GunImpuse()
    {
        if (photonView.IsMine)
        {
            Manager.Camera.PlayImpulse(1.0f, CinemachineImpulseDefinition.ImpulseShapes.Rumble);
        }
    }

    //시점 변경 테스트코드
    private void PlayerLook()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f); //상하 최대 조절 값

        _cameraHolder.transform.localEulerAngles = Vector3.left * verticalLookRotation;
    }

    //이동 테스트 코드
    private void PlayerMove()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        Vector3 move = transform.right * horizontal + transform.forward * vertical;
        transform.Translate(move * moveSpeed * Time.deltaTime, Space.World);
    }

    //rb 점프 테스트 코드
    private void PlayerJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _rb.AddForce(transform.up * 5.0f, ForceMode.Impulse);
        }
    }


    #region Setting

    //가상 카메라 및 시네머신 브레인 설정, 초기화
    public IEnumerator InitCameraSetting()
    {
        //플레이어 시야쪽 가상 캠에 VirtualCam_Base 를 추가해서 카메라 매니저에 등록
        //카메라 매니저에 stack에 push
        CinemachineVirtualCamera vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        var playerVCam = Util_LDH.GetOrAddComponent<VirtualCam_LocalPlayer>(vcam.gameObject);

        var impluseSource = Util_LDH.GetOrAddComponent<CinemachineImpulseSource>(vcam.gameObject);
        impluseSource.m_ImpulseDefinition.m_ImpulseType = CinemachineImpulseDefinition.ImpulseTypes.Uniform;

        //Manager.Camera.PushCamera(playerVCam.cameraID);
        Camera cam = GetComponentInChildren<Camera>();
        // 내 플레이어의 캠을 카메라 매니저의 매인캠으로 등록, CinemachineBrain 추가, ImpulseListener 추가
        Manager.Camera.SetMainCamera(cam);
        Manager.Camera.ApplyBlenderSettingToBrain(cam, "InGameCinemahineBlenderSetting");

        Util_LDH.GetOrAddComponent<CinemachineBrain>(cam.gameObject);
        Util_LDH.GetOrAddComponent<CinemachineIndependentImpulseListener>(cam.gameObject);

        yield return null;
    }

    #endregion


    #region Sync

    public void OnEndAnimation()
    {
        if (!PhotonNetwork.IsMasterClient)
            return;
        StartCoroutine(DelayAndTurnEnd());
    }

    private IEnumerator DelayAndTurnEnd()
    {
        Debug.Log("애니메이션 종료 후 0.5초 대기를 시작합니다.(마스터 매니저만 호출합니다.)");
        yield return new WaitForSeconds(0.5f);
        _fireSync?.RequestEndTurn();
    }

    #endregion
   
}