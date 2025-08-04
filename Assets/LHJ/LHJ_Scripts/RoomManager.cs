using ExitGames.Client.Photon;
using GameUI;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine;

/// <summary>
/// 방 내부의 플레이어 패널 관리 및 게임 시작/퇴장 로직을 담당하는 매니저
/// - 방장/클라이언트의 역할 분기
/// - 플레이어 입/퇴장 시 UI 반영
/// - Ready 상태 기반 게임 시작 가능 여부 판단
/// </summary>
public class RoomManager : MonoBehaviour
{
    //UI와 컨트롤 로직을 분리 -> 필요없는 변수 제거
    
    [SerializeField] private UI_Room _uiRoom;
    [SerializeField] private string gameSceneName;

    
    private void OnDestroy()
    {
        // 이벤트 등록 해제 (메모리 누수 방지)
        _uiRoom.OnClickStartButton -= GameStart;
        _uiRoom.OnClickLeaveButton -= LeaveRoom;
    }
    

    #region 초기화

    /// <summary>
    /// 방에 진입했을 때 UI 초기화
    /// - 방장인 경우 Start 버튼에 GameStart 기능 부여
    /// </summary>
    public void InitRoom()
    {
        if(PhotonNetwork.LocalPlayer.IsMasterClient)
            _uiRoom.Init(GameStart, LeaveRoom);      // 방장용 UI
        else
            _uiRoom.Init(null, LeaveRoom);    // 일반 유저용 UI
    }

    #endregion
    
    
    #region 게임 시작 / 퇴장

    /// <summary>
    /// 게임 시작 로직
    /// - 방장만 호출 가능
    /// - 준비 완료 여부는 버튼 활성화 조건에서 이미 확인됨
    /// </summary>
    public void GameStart()
    {
        // if (PhotonNetwork.IsMasterClient && AllPlayerReadyCheck()) 
        // -> 마스터 클라이언트에게만 Game Start 기능 부여 & 버튼 활성화에서 Ready Check를 진행하는 로직으로 변경함에 따라 주석 처리
        
        PhotonNetwork.LoadLevel(gameSceneName);
    }
    
    /// <summary>
    /// 방 나가기 로직
    /// - 본인 UI 초기화
    /// - Ready 상태 false로 리셋
    /// </summary>
    public void LeaveRoom()
    {
        // 자신의 UI만 초기화
        ResetPlayerPanel(PhotonNetwork.LocalPlayer);
        
        // 자신의 custom property 초기화
        Hashtable playerProperty = new Hashtable
        {
            { "Ready", false }
        };
        
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
        
        PhotonNetwork.LeaveRoom(); // 방 나가기
    }

    #endregion

    #region Mastaer Client 전환

    /// <summary>
    /// 방장이 변경되었을 때 호출됨
    /// - 새로운 방장이 된 클라이언트는 UI 및 Start 버튼 상태 재설정
    /// </summary>
    public void SwitchMasterClient(Player newMaster)
    {
        InitRoom();  // 새 방장의 권한에 맞게 UI 버튼 구성
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            SetPlayerPanel(player);  // 모든 플레이어 UI 다시 갱신
        }
    }

    #endregion
    
  
    #region UI

    /// <summary>
    /// 로컬/외부 플레이어 입장 시 UI 패널 생성
    /// </summary>
    public void SetPlayerPanel(Player player)
    {
        // PhotonNetwork.AutomaticallySyncScene = true;
        _uiRoom.SetPlayerPanel(player);
    }

    /// <summary>
    /// 특정 플레이어의 UI 패널 제거 (퇴장 시 호출)
    /// </summary>
    public void ResetPlayerPanel(Player player)
    {
        _uiRoom.ResetPanel(player);
    }

    /// <summary>
    /// 특정 플레이어의 Ready 상태 변경 시 호출됨
    /// - Ready UI 업데이트
    /// - Start 버튼 조건도 재평가
    /// </summary>
    public void UpdateReadyUI(Player player)
    {
        _uiRoom.UpdateReadyUI(player);
        _uiRoom.UpdateStartButtonState(AllPlayerReadyCheck());
    }

    /// <summary>
    /// 현재 방의 모든 플레이어가 Ready 상태인지 확인
    /// - 게임 시작 조건 판단용
    /// </summary>
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

    #endregion
   
 
}
