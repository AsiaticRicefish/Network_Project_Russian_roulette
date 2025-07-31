using ExitGames.Client.Photon;
using GameUI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    //UI와 컨트롤 로직을 분리 -> 필요없는 변수 제거
    
    [SerializeField] private UI_Room _uiRoom;
    [SerializeField] private string gameSceneName;

    private void OnDestroy()
    {
        _uiRoom.OnClickStartButton -= GameStart;
        _uiRoom.OnClickLeaveButton -= LeaveRoom;
    }
    

    // 개별 플레이어 패널 생성
    public void SetPlayerPanel(Player player)
    {
       // PhotonNetwork.AutomaticallySyncScene = true;
        _uiRoom.SetPlayerPanel(player);
    }

    public void InitRoom()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            _uiRoom.Init(GameStart, LeaveRoom);
        else
            _uiRoom.Init(null, LeaveRoom);
    }

    // 방장 및 모든 플레이어가 준비완료시 시작
    public void GameStart()
    {
        // if (PhotonNetwork.IsMasterClient && AllPlayerReadyCheck()) // -> 마스터 클라이언트에게만 Game Start 기능 부여 & 버튼 활성화에서 Ready Check를 진행하는 로직으로 변경함에 따라 주석 처리
        PhotonNetwork.LoadLevel(gameSceneName);
    }
    
    public void LeaveRoom()
    {
        // foreach (var player in PhotonNetwork.PlayerList)
        // {
        //     _uiRoom.ResetPanel(player);
        // }
        ResetPlayerPanel(PhotonNetwork.LocalPlayer);
        Hashtable playerProperty = new Hashtable
        {
            { "Ready", false }
        };
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
        
        PhotonNetwork.LeaveRoom(); // 방 나가기
    }
    
    // 나간 플레이어의 패널 제거
    public void ResetPlayerPanel(Player player)
    {
        _uiRoom.ResetPanel(player);
    }

    public void SwitchMasterClient(Player newMaster)
    {
        InitRoom();
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            SetPlayerPanel(player);
        }
    }
    
    public void UpdateReadyUI(Player player)
    {
        _uiRoom.UpdateReadyUI(player);
        _uiRoom.UpdateStartButtonState(AllPlayerReadyCheck());
    }
    

    // 모든플레이어가 준비완료 상태인지
    public bool AllPlayerReadyCheck()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            bool ready = player.CustomProperties.TryGetValue("Ready", out object value) && (bool)value;
            Debug.Log($"[ReadyCheck] {player.NickName} Ready = {ready}");
            if (!ready)
                return false;
        }
        return true;
    }

   
 
}
