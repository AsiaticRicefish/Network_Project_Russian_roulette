using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class GamePlayer : MonoBehaviour
{
    private PhotonView _pv;

    [SerializeField] private PlayerData _data;
    public int AssignedSpawnPointIndex { get; private set; } = -1; // 할당된 스폰 지점 인덱스 (-1은 할당되지 않음을 의미)
    public string Nickname => _data.nickname;
    public string PlayerId => _data.playerId;
    public int MaxHp => _data.maxHp;
    public int CurrentHp => _data.currentHp;
    public bool IsAlive => _data.isAlive;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    /// <summary>
    /// 플레이어 데이터를 초기화하고, 스폰 지점 인덱스를 저장하는 RPC
    /// </summary>
    /// <param name="nickname"></param>
    /// <param name="firebaseUID"></param>
    /// <param name="winCount"></param>
    /// <param name="loseCount"></param>
    /// <param name="initialSpawnPointIndex">이 플레이어가 스폰된 스폰 지점의 인덱스</param>
    [PunRPC]
    public void RPC_InitializePlayer(string nickname, string firebaseUID, int winCount, int loseCount, int initialSpawnPointIndex)
    {
        _data = new PlayerData(nickname, firebaseUID, winCount, loseCount);
        _data.maxHp = 3; // 예시 HP
        _data.currentHp = _data.maxHp;
        _data.isAlive = true;
        AssignedSpawnPointIndex = initialSpawnPointIndex; // 스폰 지점 인덱스 저장

        Debug.Log($"Initialized GamePlayer: {Nickname}, HP: {CurrentHp} at spawn index {AssignedSpawnPointIndex}");
        // UIManager 등을 통해 플레이어 UI 업데이트 로직 호출 가능
    }

    private void Start()
    {
        //Debug.Log($"{Nickname},{PlayerId}");
    }

    public void Initialize(PlayerData data)
    {
        _data = data;
    }

    /// <summary>
    /// Player MaxHp Setting 메서드
    /// </summary>
    /// <param name="newHp"> 설정 시킬 MaxHp값 </param>
    public void SetMaxHp(int newHp)
    {
        if (newHp < 0)
        {
            Debug.LogError("0보다 작은 값으로 MaxHp 설정 불가");
            return;
        }
        _data.maxHp = newHp;
    }

    /// <summary>
    /// Player hp 관련 메서드
    /// </summary>
    /// <param name="amount"></param>
    public void IncreaseHp(int amount)
    {
        _data.currentHp = Mathf.Min(_data.currentHp + amount, _data.maxHp);
    }

    public void DecreaseHp(int amount)
    {
        _data.currentHp = Mathf.Max(_data.currentHp - amount, 0);
        if (_data.currentHp <= 0)
            _data.isAlive = false;
    }

    //Player에서 PlayerData를 넘겨주는 메서드
    public PlayerData ToPlayerData()
    {
        return _data;
    }
}
