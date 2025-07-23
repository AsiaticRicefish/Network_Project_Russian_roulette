using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GamePlayer : MonoBehaviour
{
    [SerializeField] private PlayerData _data;
    public string Nickname => _data.nickname;
    public string PlayerId => _data.playerId;
    public int MaxHp => _data.maxHp;
    public int CurrentHp => _data.currentHp;
    public bool IsAlive => _data.isAlive;


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
