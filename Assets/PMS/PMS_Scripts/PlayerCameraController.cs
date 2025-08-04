using UnityEngine;
using Cinemachine;
using Photon.Pun;

public class PlayerCameraController : MonoBehaviourPun
{
    [Header("References")]
    [SerializeField] public CinemachineBrain _cinemachineBrain;
    [SerializeField] private Transform _cameraRig;
    [SerializeField] private float _minPitch;
    [SerializeField] private float _maxPitch;
    [SerializeField] private CinemachineVirtualCamera _playerCam;

    [Header("Mouse Config")]
    [SerializeField][Range(0, 1)] public float MouseXSensitivity;

    [SerializeField][Range(0, 1)] public float MouseYSensitivity;

    public float OffsetX { get; private set; }
    public float OffsetY { get; private set; }
    public bool CameraMove { get; private set; }


    private void Awake()
    {
        CameraMove = true;
        Init();
    }

    private void Start()
    {
        if (!photonView.IsMine)
        {
            //자기 카메라가 아니면 다 비활성화 처리
            //추후 게임이 종료되도 관전 모드를 만들게 되면 다른 유저의 시야를 봐야 할 수도 있기 때문에
            GetComponentInChildren<Camera>().gameObject.SetActive(false);
        }
    }

    private void Init()
    {
        //기본 초기값 들고올 곳 있으면 들고오기
    }

    private void Update()
    {
        //if (!photonView.IsMine) return;

        //PlayerLook();
    }

    private void PlayerLook()
    {
        if (!CameraMove)
            return;

        OffsetX += Input.GetAxis("Mouse X") * MouseXSensitivity;
        OffsetY -= Input.GetAxis("Mouse Y") * MouseYSensitivity;
        OffsetX = Mathf.Clamp(OffsetX, _minPitch, _maxPitch);
        OffsetY = Mathf.Clamp(OffsetY, _minPitch, _maxPitch);
        _cameraRig.localEulerAngles = new Vector3(OffsetY, OffsetX, 0f);
    }
}
