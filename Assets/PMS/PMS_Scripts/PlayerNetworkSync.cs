using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

//MonoBehaviourPun을 상속하면 따로 PhotonView를 캐싱하지 않아도  해당 게임 오브젝트에 붙어있는 PhotonView 컴포넌트에 접근 가능
public class PlayerNetworkSync : MonoBehaviourPun
{
    // 동기화할 대상인 나의 GamePlayer 컴포넌트 
    private GamePlayer _gamePlayer;

    //여기서 게임 컴포넌트 초기화
    private void Awake()
    {
        // 같은 GameObject에 있는 GamePlayer 컴포넌트를 가져옵니다.
        _gamePlayer = GetComponent<GamePlayer>();
        if (_gamePlayer == null)
        {
            Debug.LogError("GamePlayer 컴포넌트가 할당되어 있지 않습니다.");
        }
    }

    private void OnEnable()
    {
        // GamePlayer의 이벤트를 구독하여 hp 변경시 hp 및 Alive 값 전달 RPC 호출
        if (_gamePlayer != null)
        {
            _gamePlayer.OnHpChanged += OnGamePlayerCurrentHpChanged;
        }
    }

    private void OnDisable()
    {
        // 구독 해제 (메모리 누수 방지)
        if (_gamePlayer != null)
        {
            _gamePlayer.OnHpChanged -= OnGamePlayerCurrentHpChanged;
        }
    }

    private void OnGamePlayerCurrentHpChanged(int currentHp, bool isAlive)
    {
        // 내가 소유한 GamePlayer의 체력만 동기화
        if (photonView.IsMine)
        {
            // 모든 클라이언트에 체력과 생존 상태 업데이트 RPC 전송
            //현재 체력과 현재 생존 상태를 전달
            photonView.RPC("RPC_UpdatePlayerCurrentHp", RpcTarget.Others, currentHp, isAlive);
            Debug.Log($"[PlayerNetworkSync] 내 HP 변경 RPC 호출: CurrentHp : {currentHp}, IsAlive : {isAlive}");
        }
    }

    //플레이어의 MaxHp변동에 따른 hp값과 그에 따른 IsAlive값 동기화를 위한 RPC함수 - MaxHp를 조정한다는 것은 라운드가 끝나서 초기화 되야 한다는 것을 의미, 플레이어를 다시 살려야한다.
    //모든 플레이어들을 살려내야한다.
    [PunRPC]
    public void RPC_UpdatePlayerMaxHp(int newMaxHp, bool newIsAlive, PhotonMessageInfo info)
    {
        // RPC를 보낸 클라이언트가 이 PhotonView의 Owner인지 확인 (보안 및 유효성)
        if (info.Sender.ActorNumber != photonView.Owner.ActorNumber)
        {
            Debug.LogWarning($"[PlayerNetworkSync] RPC_UpdatePlayerState RPC 호출자가 이 PhotonView의 Owner가 아닙니다! Sender: {info.Sender.NickName}, Owner: {photonView.Owner.NickName}");
            return;
        }

        // GamePlayer의 속성 Setter를 통해 상태 업데이트 (Setter 내부에서 이벤트 발생)
        _gamePlayer.MaxHp = newMaxHp;
        _gamePlayer.IsAlive = newIsAlive; // 체력이 0이 되어 IsAlive가 false로 바뀌는 것도 동기화

        Debug.Log($"[PlayerNetworkSync] RPC 수신: {info.Sender.NickName} ({_gamePlayer.PlayerId})의 상태 업데이트: MaxHP : {_gamePlayer.CurrentHp}, Alive : {_gamePlayer.IsAlive}");
    }

    //플레이어의 hp변동에 따른 hp값과 그에 따른 IsAlive값 동기화를 위한 RPC함수 - 호출 예상 타이밍(플레이어hp 관련 메서드 호출시, ex)맥주를 통한 피회복,총을 통한 데미지 입음)
    [PunRPC]
    public void RPC_UpdatePlayerCurrentHp(int newHp, bool newIsAlive, PhotonMessageInfo info)
    {
        // RPC를 보낸 클라이언트가 이 PhotonView의 Owner인지 확인 (보안 및 유효성)
        if (info.Sender.ActorNumber != photonView.Owner.ActorNumber)
        {
            Debug.LogWarning($"[PlayerNetworkSync] RPC_UpdatePlayerState RPC 호출자가 이 PhotonView의 Owner가 아닙니다! Sender: {info.Sender.NickName}, Owner: {photonView.Owner.NickName}");
            return;
        }

        // GamePlayer의 속성 Setter를 통해 상태 업데이트 (Setter 내부에서 이벤트 발생)
        _gamePlayer.CurrentHp = newHp;
        _gamePlayer.IsAlive = newIsAlive; // 체력이 0이 되어 IsAlive가 false로 바뀌는 것도 동기화

        Debug.Log($"[PlayerNetworkSync] RPC 수신: {info.Sender.NickName} ({_gamePlayer.PlayerId})의 상태 업데이트: CurrentHP : {_gamePlayer.CurrentHp}, Alive : {_gamePlayer.IsAlive}");
    }
}
