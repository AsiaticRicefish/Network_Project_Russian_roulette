using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerController : MonoBehaviour
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

        if(Input.GetKeyDown(KeyCode.Space))
        {
            //PlayerManager.Instance.PlayerListPrint();
            //Debug.Log(PlayerManager.Instance.GetAllPlayers().Count);
            Debug.Log(PlayerManager.Instance.GetAllPlayers().Count);
        }

        PlayerLook();
        PlayerMove();
        PlayerJump();

        //TODO - 이후 GameManager에서 턴이 시작되면 만약 해당 턴이 내가 가질 턴이면 아이템 사용 이나 Gun사용 활성화 처리가 필요
        //bool 값으로 전달 받으면 좋을 것 같다.

        /*if (GameManager.Instance.StartTurn())
        {
            photonView.RPC(nameof(Shoot), RpcTarget.All);
        }*/
    }

    [PunRPC]
    private void Shoot()
    {
        // 총알 종류에 따라 효과 적용
        Debug.Log("총 발사!");
        //GunManager.Instance.Fire(Player player);
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
