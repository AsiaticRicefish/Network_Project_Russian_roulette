using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class GamePlayer : MonoBehaviour, IComparer<GamePlayer>
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
    //public int AssignedSpawnPointIndex { get; private set; } = -1; // 할당된 스폰 지점 인덱스 (-1은 할당되지 않음을 의미)

    //플레이어 인게임 프로퍼티
    //Maxhp를 수정할때에는 플레이어가 살아야 할 때 밖에 없지 않을까? 이걸IsAlive를 동기화하는 로직이 따로 있어야할까?
    public int MaxHp
    {
        get { return _maxHp; }
        set
        {
            // 최대 체력 변경 시 이벤트 발생 로직 추가
            if (_maxHp != value) // 실제 값이 변경될 때만 이벤트 발생
            {
                _maxHp = value;
                _isAlive = true;
                OnMaxHpChanged?.Invoke(_maxHp, IsAlive);
            }
        }
    }

    public int CurrentHp
    {
        get { return _currentHp; }
        set
        {
            // 현재 체력 변경 시 이벤트 발생 로직 추가
            if (_currentHp != value) // 실제 값이 변경될 때만 이벤트 발생
            {
                _currentHp = value;
                if (CurrentHp <= 0) IsAlive = false;

                OnHpChanged?.Invoke(_currentHp,IsAlive);
            }
        }
    }
    public bool IsAlive
    {
        get { return _isAlive; }
        set
        {
            // 생존 여부 변경 시 이벤트 발생 로직 추가
            if (_isAlive != value) // 실제 값이 변경될 때만 이벤트 발생
            {
                _isAlive = value;
                if (!value) // 사망 시
                {
                    OnPlayerDied?.Invoke(this); // 사망한 플레이어 객체 전달
                }
            }
        }
    }

    //이벤트 
    public event Action<int,bool> OnHpChanged;           //최대 체력 변경시 - 라운드가 종료 되었을 때
    public event Action<int,bool> OnMaxHpChanged;         //현재 체력 변경시 - 총을 맞거나 피를 회복할때

    public event Action<GamePlayer> OnPlayerDied;   // 사망한 플레이어 객체 전달 - 턴관리 넘겨야하는데

    public bool IsCuffedThisTurn = false;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        if (_pv.IsMine)
        {
            //테스트 코드
            //일단 지금 당장 순서 보장해주기 힘드니깐 정렬을 사용해보자
            StartCoroutine(PlayerListSortDelay());           
            Initialize();
        }
    }

    public void Initialize()
    {
        //프로퍼티 사용해서 이벤트 호출하게 하여야함. 임시 테스트 코드
        MaxHp = 3;
        CurrentHp = MaxHp;
        IsAlive = true;
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

    #region 플레이어 hp 관련 메서드
    public void IncreaseHp(int amount)
    {
        if (!IsAlive) return;

        CurrentHp += amount;
        CurrentHp = Mathf.Max(CurrentHp, 0); // CurrentHp setter에서 사망 처리
    }

    public void DecreaseHp(int amount)
    {
        if (!IsAlive) return;

        CurrentHp -= amount;
        CurrentHp = Mathf.Min(CurrentHp, MaxHp);
    }
    #endregion

    //Player에서 PlayerData를 넘겨주는 메서드 - 필요하진 모르겟다 
    public PlayerData ToPlayerData()
    {
        return _data;
    }

    #region OnPhotonSerializeView 사용 테스트
    /*public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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
    }*/
    #endregion

    //receiveSpawnPointIndex 범위 0 ~ 플레이어수-1  -> 이값을 가지고 List를 넣는 순서를 제어해도 괜찮을 것 같다. 이유 : 일단 자리에 앉으면 오른쪽으로 도는형식, 1대1에서 의미가 없지만 3인이상일 경우 턴순서를 확인할 수 있다.
    // PlayerData 객체를 직접 전송하는 RPC 함수
    [PunRPC]
    public void ReceivePlayerData(string receivedNickname, string receivedPlayerId, int receivedWinCount, int receivedLoseCount, int receiveSpawnPointIndex)
    {
        // 수신된 데이터를 사용하여 플레이어 업데이트
        _data = new PlayerData(receivedNickname, receivedPlayerId, receivedWinCount, receivedLoseCount);
        _spawnPointindex = receiveSpawnPointIndex; 
        Debug.Log($"RPC로 수신된 플레이어 닉네임: {_data.nickname}, 플레이어 ID: {_data.playerId}, 승리: {_data.winCount}, 패배: {_data.loseCount}");
        PlayerManager.Instance.RegisterPlayer(this);

        // TODO - 모든 플레이어가 List의 순서를 보장해줘야한다.
        PlayerManager.Instance._playerList.Add(this);
    }

    // 내 PlayerData를 다른 클라이언트에게 보내는 함수
    public void SendMyPlayerDataRPC()
    {
        // RpcTarget.AllViaServer는 모든 클라이언트에게 RPC를 전송  전송자 -> 서버 -> 클라이언트 RpcTarget.AllViaServer
        _pv.RPC("ReceivePlayerData", RpcTarget.All, _data.nickname,_data.playerId,_data.winCount,_data.loseCount,_spawnPointindex);
    }

    //테스트 코드 임시 스폰 pos에 따른 Compare구현 - 각자의 List Sort하기 위해서
    public int Compare(GamePlayer x, GamePlayer y)
    {
        return x._spawnPointindex.CompareTo(y._spawnPointindex);
    }

    private IEnumerator PlayerListSortDelay()
    {
        yield return new WaitForSeconds(0.1f);
        PlayerManager.Instance._playerList.Sort(Compare);
    }
}
