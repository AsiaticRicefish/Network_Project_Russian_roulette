using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : Singleton<PlayerManager>
{ 
    private List<Player> _players;
    public List<Player> Players => _players;

    //리스트에서 플레이어를 지우거나 추가를 할 때 자동 동기화 될 수 있도록 하는방법? 
    //public List<Player> Players { get { return _players} set { } };
    // TODO - 나중에 Manager 등록하게 해줘야한다.
    private void Awake()
    {
        SingletonInit();
        _players = new List<Player>(); // _players 리스트 초기화
    }

    public void AddPlayer(Player player)
    {
        if (!_players.Contains(player))
        {
            _players.Add(player);
            Debug.Log($"Player added! Player FirebaseUID Number : {player.FirebaseUid}. Current players: {_players.Count}");
        }
        else
        {
            Debug.LogWarning($"Player ID : {player.FirebaseUid} 에 해당되는 유저가 이미 존재 합니다.");
        }
    }

    public void RemovePlayer(Player player)
    {
        if (_players.Contains(player))
        {
            _players.Remove(player);
        }
        else
        {
            Debug.LogWarning($"Player ID : {player.FirebaseUid}에 해당되는 유저가 존재하지 않습니다.");
        }
    }

}
