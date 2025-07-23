using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private PlayerData _data;

    public string _playerId;
    public string _firebaseUid;
    public int _maxHp;
    public int _currentHp;
    public bool _isAlive;
    //public List<ItemData> _itemslot;

    public string PlayerId { get { return _playerId; } }
    public int MaxHp { get { return _maxHp; } }
    public int CurrentHp { get { return _currentHp; } }
    public bool IsAlive { get { return _isAlive; } }

    public void Initialize(PlayerData data)
    {
        _data = data;
        _playerId = _data.playerId;
        _firebaseUid = _data.firebaseUid;
        _maxHp = _data.maxHp;
        _currentHp = _data.currentHp;
        _isAlive = _data.isAlive;
    }

    //추가적인 네트워크 동기화 작업이 필요

    /// <summary>
    /// 라운드 초기화 할 때 각자 플레이어의 MaxHp가 달라져야 한다
    /// </summary>
    /// <param name="newhp"> maxhp로 변경될 값 </param>
    public void SetMaxHp(int newhp)
    {
        if (newhp < 0)
        {
            Debug.LogError("0보다 작은 값으로 플레이어의 MaxHp를 셋팅 할 수 없습니다");
            return;
        }

        _maxHp = newhp;
    }

    /// <summary>
    /// Hp 관련 아이템 사용 및 총 맞았을 때 사용 될 함수
    /// </summary>
    /// <param name="amount"> hp 변동 숫자 </param>
    public void IncreaseHp(int amount) 
    {
        _currentHp = Mathf.Min(_currentHp + amount, _maxHp);
    }
    public void DecreaseHp(int amount) 
    {
        _currentHp = Mathf.Max(_currentHp - amount, 0);
        if(_currentHp <= 0)
        {
            _isAlive = false;
        }
    }
    
}
