using GameUI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


/// <summary>
/// 로비 UI에서 각 방 항목을 나타내는 클래스.
/// 방 정보를 표시하고, 선택/입장 기능을 담당한다.
/// </summary>
public class RoomList : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button roomListButton;

    [Header("Colors")]
    [SerializeField] private Color _selectedColor;
    private Color _originColor;
    
    
    private string _roomName;           // 실제 Photon room name (식별용)
    private string _userRoomName;       // 유저에게 표시되는 이름 (커스텀 프로퍼티)
    private string _roomCode;           // 고유 코드
    private RoomInfo _info;             // Photon room 정보
    private UI_Lobby _lobby;            // 소속된 로비 UI 참조

    public string RoomCode => _roomCode;     // 외부 접근용 프로퍼티

    private void Start()
    {
        _originColor = roomListButton.image.color;
    }
    
    /// <summary>
    /// 방 리스트 항목을 초기화합니다. (데이터, UI 초기화)
    /// </summary>
    /// <param name="info">Photon 방 정보</param>
    /// <param name="lobby">소속 로비 UI</param>
    public void Init(RoomInfo info, UI_Lobby lobby)
    {
        //------ data 초기화 -----//
        _info = info;
        _lobby = lobby;
        
        _roomName = _info.Name;
        _userRoomName = info.CustomProperties["userRoomName"] as string;
        _roomCode = info.CustomProperties["roomCode"] as string;
        
        //------ ui 초기화 -----//
        roomNameText.text = $"{_userRoomName}";
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        statusText.text = "Waiting"; //todo: status Text (상태값을 동적으로 표시하기 위해서 커스텀 프로퍼티 추가해야 함)
        
        //---- 이벤트 등록 -----//
        roomListButton.onClick.AddListener(SelectRoom);
    }

    private void OnDestroy()
    {
        // 삭제 시 선택 해제 및 이벤트 제거
        _lobby.OnRoomSelected(null);
        roomListButton.onClick.RemoveListener(SelectRoom);
    }
    
    /// <summary>
    /// 해당 방 입장
    /// </summary>
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRoom(_roomName);
            
        }
        
    }

    #region Select Event

    /// <summary>
    /// 방 항목 클릭 시 호출. 선택 상태 토글 처리.
    /// </summary>
    public void SelectRoom()
    {
        if (_lobby.SelectedRoom == this)
        {
            // 이미 선택된 상태 → 선택 해제
            _lobby.OnRoomSelected(null);
        }
        else
        {
            // 새로운 방 선택
            _lobby.OnRoomSelected(this);
            Debug.Log($"Room Name : {_roomName}, user room name : {_userRoomName}");
        }
    }

    /// <summary>
    /// 선택 상태에 따라 색상을 갱신합니다.
    /// </summary>
    /// <param name="isSelected">선택 여부</param>
    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            roomListButton.image.color = _selectedColor;
        }
        else
        {
            roomListButton.image.color = _originColor;
        }
    }


    #endregion
  
    
}
