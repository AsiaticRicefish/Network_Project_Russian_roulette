using Cinemachine;
using GameCamera;
using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Utils;

public class PlayerController : MonoBehaviourPun
{
    [SerializeField] private Animator _animator;
    [SerializeField] private GameObject _cameraHolder;
    [SerializeField] private float _mouseSensitivity;

    private float moveSpeed = 5.0f;
    private float verticalLookRotation;

    private PhotonView _pv;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
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


    private void Update()
    {
        if (!_pv.IsMine) return;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _animator.SetTrigger("Shot");
        }
        //PlayerLook();
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

    //가상 카메라 및 시네머신 브레인 설정, 초기화
    public IEnumerator InitCameraSetting()
    {
        //플레이어 시야쪽 가상 캠에 VirtualCam_Base 를 추가해서 카메라 매니저에 등록
        //카메라 매니저에 stack에 push
        CinemachineVirtualCamera vcam = GetComponentInChildren<CinemachineVirtualCamera>();
        var playerVCam = Util_LDH.GetOrAddComponent<VirtualCam_LocalPlayer>(vcam.gameObject);
        Manager.Camera.PushCamera(playerVCam.cameraID);

        Camera cam = GetComponentInChildren<Camera>();
        // 내 플레이어의 캠을 카메라 매니저의 매인캠으로 등록, CinemachineBrain 추가, ImpulseListener 추가
        Manager.Camera.SetMainCamera(cam);
        Manager.Camera.ApplyBlenderSettingToBrain(cam, "InGameCinemahineBlenderSetting");

        Util_LDH.GetOrAddComponent<CinemachineBrain>(cam.gameObject);
        Util_LDH.GetOrAddComponent<CinemachineIndependentImpulseListener>(cam.gameObject);

        yield return null;
    }
}