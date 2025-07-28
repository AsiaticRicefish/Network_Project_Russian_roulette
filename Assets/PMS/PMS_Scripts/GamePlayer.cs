using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GamePlayer : MonoBehaviour
{
    public PhotonView _pv;

    public PlayerData _data;

    //데이터를 읽기는 해야하는데 Write하면 안되는 데이터
    public string Nickname => _data.nickname;       
    public string PlayerId => _data.playerId;

    //읽고 써야 하는 데이터
    private int _maxHp;
    private int _currentHp;
    private bool _isAlive;

    public int _spawnPointindex = -1;

    public int MaxHp { get { return _maxHp; } }
    public int CurrentHp { get { return _currentHp; } }
    public bool IsAlive { get { return _isAlive; } }

    //public int AssignedSpawnPointIndex { get; private set; } = -1; // 할당된 스폰 지점 인덱스 (-1은 할당되지 않음을 의미)

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    //플레이어 생성 -> 게임 매니저에 있어야할 것같은데

    public void Initialize(PlayerData data)
    {
        _data = data;
    }

    public void SetMaxHp(int newHp)
    {
        if (newHp < 0)
        {
            Debug.LogError("0보다 작은 값으로 MaxHp 설정 불가");
            return;
        }
        _maxHp = newHp;
    }

    public void IncreaseHp(int amount)
    {
        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
    }

    public void DecreaseHp(int amount)
    {
        _currentHp = Mathf.Max(_currentHp - amount, 0);
        if (_currentHp <= 0)
            _isAlive = false;
    }

    //Player에서 PlayerData를 넘겨주는 메서드 - 필요하진 모르겟다 
    public PlayerData ToPlayerData()
    {
        return _data;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            // 자신의 데이터를 다른 클라이언트로 전송
            stream.SendNext(_maxHp);
            stream.SendNext(_currentHp);
            stream.SendNext(_isAlive);
        }
        else
        {
            // 다른 클라이언트로부터 데이터를 수신하여 적용
            _maxHp = (int)stream.ReceiveNext();
            _currentHp = (int)stream.ReceiveNext();
            _isAlive = (bool)stream.ReceiveNext();
            Debug.Log($"보낸 플레이어 : {info.Sender.NickName} , 보낸 서버 시간 : {info.SentServerTime}"); 
        }
    }

    //receiveSpawnPointIndex 범위 0 ~ 플레이어수-1  -> 이값을 가지고 List를 넣는 순서를 제어해도 괜찮을 것 같다. 이유 : 일단 자리에 앉으면 오른쪽으로 도는형식, 1대1에서 의미가 없지만 3인이상일 경우 턴순서를 확인할 수 있다.
    // PlayerData 객체를 직접 전송하는 RPC 함수
    [PunRPC]
    public void ReceivePlayerData(string receivedNickname, string receivedPlayerId, int receivedWinCount, int receivedLoseCount, int receiveSpawnPointIndex)
    {
        // 수신된 데이터를 사용하여 플레이어 업데이트
        _data = new PlayerData(receivedNickname, receivedPlayerId, receivedWinCount, receivedLoseCount);
        _spawnPointindex = receiveSpawnPointIndex; 
        Debug.Log($"RPC로 수신된 플레이어 닉네임: {_data.nickname}, 플레이어 ID: {_data.playerId}, 승리: {_data.winCount}, 패배: {_data.loseCount}"); 
    }

    // 내 PlayerData를 다른 클라이언트에게 보내는 함수
    public void SendMyPlayerDataRPC()
    {
        // RpcTarget.AllViaServer는 모든 클라이언트에게 RPC를 전송  전송자 -> 서버 -> 클라이언트 RpcTarget.AllViaServer
        _pv.RPC("ReceivePlayerData", RpcTarget.All, _data.nickname,_data.playerId,_data.winCount,_data.loseCount);
    }
}
