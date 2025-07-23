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

    //�߰����� ��Ʈ��ũ ����ȭ �۾��� �ʿ�

    /// <summary>
    /// ���� �ʱ�ȭ �� �� ���� �÷��̾��� MaxHp�� �޶����� �Ѵ�
    /// </summary>
    /// <param name="newhp"> maxhp�� ����� �� </param>
    public void SetMaxHp(int newhp)
    {
        if (newhp < 0)
        {
            Debug.LogError("0���� ���� ������ �÷��̾��� MaxHp�� ���� �� �� �����ϴ�");
            return;
        }

        _maxHp = newhp;
    }

    /// <summary>
    /// Hp ���� ������ ��� �� �� �¾��� �� ��� �� �Լ�
    /// </summary>
    /// <param name="amount"> hp ���� ���� </param>
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
