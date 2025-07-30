using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private Button roomListButton;
    [SerializeField] private TextMeshProUGUI statusText;

    private string _roomName;
    private string _userRoomName;
    private string _roomCode;
    private RoomInfo _info;
    private Transform _parent;

    public string RoomCode => _roomCode;
    
    
    // 방 정보 UI 초기화
    public void Init(RoomInfo info)
    {
        _info = info;
        _roomName = _info.Name;
        _userRoomName = (string)info.CustomProperties["userRoomName"];
        roomNameText.text = $"{_userRoomName}";
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        //todo: status Text
        statusText.text = "Waiting";
        //joinButton.onClick.AddListener(JoinRoom);
    }

    // 해당 방 입장
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.JoinRoom(_roomName);
        //joinButton.onClick.RemoveListener(JoinRoom);
    }
}
