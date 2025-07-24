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
    [SerializeField] private Button joinButton;

    private string roomName;

    // 방 정보 UI 초기화
    public void Init(RoomInfo info)
    {
        roomName = info.Name;
        roomNameText.text = $"{roomName}";
        playerCountText.text = $"{info.PlayerCount} / {info.MaxPlayers}";
        joinButton.onClick.AddListener(JoinRoom);
    }

    // 해당 방 입장
    public void JoinRoom()
    {
        if (PhotonNetwork.InLobby)
            PhotonNetwork.JoinRoom(roomName);
        joinButton.onClick.RemoveListener(JoinRoom);
    }
}
