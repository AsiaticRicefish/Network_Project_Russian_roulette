using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviourPun
{
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
        if(!_pv.IsMine)
        {
            //자기 카메라가 아니면 다 비활성화 처리
            //추후 게임이 종료되도 관전 모드를 만들게 되면 다른 유저의 시야를 봐야 할 수도 있기 때문에
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
            Destroy(_rb);
        }
        
        //player manager에 player를 등록한다.
        var gamePlayer = GetComponent<GamePlayer>();
        Manager.PlayerManager.RegisterPlayer(gamePlayer);
    }

    private void Update()
    {
        if (!_pv.IsMine) return;

        PlayerLook();
    }

    //시점 변경 테스트코드
    private void PlayerLook()
    {
        transform.Rotate(Vector3.up * Input.GetAxisRaw("Mouse X") * _mouseSensitivity);

        verticalLookRotation += Input.GetAxisRaw("Mouse Y") * _mouseSensitivity;
        verticalLookRotation = Mathf.Clamp(verticalLookRotation, -90f, 90f);            //상하 최대 조절 값

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
}
