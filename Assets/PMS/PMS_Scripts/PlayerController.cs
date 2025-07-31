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
    }

    private void Update()
    {
        if (!_pv.IsMine) return;

        PlayerLook();
    }

    // 해당 함수는 내가 작동시키나 RPC함수의 실행은 모든클라이언트에서 이루어져야함 -> RPCTarget.All로 설정
    public void AttackPlayer(GamePlayer targetPlayer, int damage)
    {
        // 내가 이 공격을 수행하는 주체일 때만 RPC 호출
        if (photonView.IsMine)
        {
            // 타겟 플레이어의 ID를 RPC로 전송하여 모든 클라이언트에서 HP 감소 로직을 실행
            photonView.RPC("RPC_DecreasePlayerHp", RpcTarget.All, targetPlayer.PlayerId, damage);
        }
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
