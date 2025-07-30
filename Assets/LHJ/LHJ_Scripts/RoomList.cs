using GameUI;
using Photon.Pun;
using Photon.Realtime;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering.VirtualTexturing;
using UnityEngine.UI;

public class RoomList : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI roomNameText;
    [SerializeField] private TextMeshProUGUI playerCountText;
    [SerializeField] private TextMeshProUGUI statusText;
    [SerializeField] private Button roomListButton;

    private string _roomName;
    private string _userRoomName;
    private string _roomCode;
    private RoomInfo _info;
    private UI_Lobby _lobby;

    public string RoomCode => _roomCode;
    
    
    // 방 정보 UI 초기화
    public void Init(RoomInfo info, UI_Lobby lobby)
    {
        _info = info;
        _lobby = lobby;
        
        _roomName = _info.Name;
        _userRoomName = (string)info.CustomProperties["userRoomName"];
        roomNameText.text = $"{_userRoomName}";
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        //todo: status Text
        statusText.text = "Waiting";
        
        roomListButton.onClick.AddListener(SelectRoom);
    }

    private void OnDestroy()
    {
        roomListButton.onClick.RemoveListener(SelectRoom);
    }

    public void SelectRoom()
    {
        _lobby.OnRoomSelected(this);
        Debug.Log($"Room Name : {_roomName}, user room name : {_userRoomName}");
    }

    public void SetSelected(bool isSelected)
    {
        if (isSelected)
        {
            roomListButton.Select();
        }
        else
        {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }

    // 해당 방 입장
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
        {
            PhotonNetwork.JoinRoom(_roomName);
            
        }
        
    }
}
