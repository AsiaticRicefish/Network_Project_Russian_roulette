using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PMS_Test
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        private List<GamePlayer> _players;
        public List<GamePlayer> Players => _players;

        //리스트에서 플레이어를 지우거나 추가를 할 때 자동 동기화 될 수 있도록 하는방법? 
        //public List<Player> Players { get { return _players} set { } };
        // TODO - 나중에 Manager 등록하게 해줘야한다.
        private void Awake()
        {
            SingletonInit();
            _players = new List<GamePlayer>(); // _players 리스트 초기화
        }

        public void AddPlayer(GamePlayer player)
        {
            if (!_players.Contains(player))
            {
                _players.Add(player);
                Debug.Log($"Player added! Player FirebaseUID Number : {player.PlayerId}. Current players: {_players.Count}");
            }
            else
            {
                Debug.LogWarning($"Player ID : {player.PlayerId} 에 해당되는 유저가 이미 존재 합니다.");
            }
        }

        public void RemovePlayer(GamePlayer player)
        {
            if (_players.Contains(player))
            {
                _players.Remove(player);
            }
            else
            {
                Debug.LogWarning($"Player ID : {player.PlayerId}에 해당되는 유저가 존재하지 않습니다.");
            }
        }

        //Player 고유값인 uid로 리스트에서 삭제하기 
        public bool RemovePlayerByUID(string uid)
        {
            GamePlayer player = FindPlayerByUID(uid);
            if (player != null)
            {
                _players.Remove(player);
                return true;
            }
            else
            {
                Debug.LogWarning($"Player UID: {uid} 가 리스트에 없습니다.");
                return false;
            }
        }

        public GamePlayer FindPlayerByUID(string uid)
        {
            return _players.Find(p => p.PlayerId == uid);
        }

        public void PlayerListPrint()
        {
            foreach (var player in _players)
            {
                Debug.Log(player.Nickname);
            }
        }
    }
}
