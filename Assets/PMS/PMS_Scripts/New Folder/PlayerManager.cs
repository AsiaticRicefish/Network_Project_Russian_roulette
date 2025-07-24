using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; // ActorNumber를 사용하기 위해 추가
using Photon.Realtime;

namespace PMS_Test
{
    public class PlayerManager : Singleton<PlayerManager>
    {
        //리스트가 아닌 Dictionary로 관리 Key - FirebaseUID, Player GamePlayer  
        private Dictionary<string, GamePlayer> _players;

        public Dictionary<string, GamePlayer> GetAllPlayers() => _players;

        //리스트에서 플레이어를 지우거나 추가를 할 때 자동 동기화 될 수 있도록 하는방법? 
        //public List<Player> Players { get { return _players} set { } };
        // TODO - 나중에 Manager 등록하게 해줘야한다.
        private void Awake()
        {
            SingletonInit();
            if (PhotonNetwork.IsMasterClient)
            {
                Init();
            }
        }

        private void Init()
        {
            _players = new Dictionary<string, GamePlayer>();
        }

        //플레이어 UID로 관리 되는 딕셔너리에 등록
        public void RegisterGamePlayer(Player photonPlayer)
        {
            string uid = photonPlayer.CustomProperties["uid"]?.ToString() ?? "unknown";
            string nickname = photonPlayer.NickName;

            GamePlayer gamePlayer = new GamePlayer();
            gamePlayer.Initialize(uid, nickname);
            RegisterPlayer(gamePlayer);
        }

        public void RegisterPlayer(GamePlayer player)
        {
            if (!_players.ContainsKey(player.PlayerId))
            {
                _players.Add(player.PlayerId, player);
                Debug.Log($"Player added! Player FirebaseUID Number : {player.PlayerId}. Current players: {_players.Count}");
            }
            else
            {
                Debug.LogWarning($"Player ID : {player.PlayerId} 에 해당되는 유저가 이미 존재 합니다.");
            }
        }

        public void RemovePlayer(GamePlayer player)
        {
            if (_players.ContainsKey(player.PlayerId))
            {
                _players.Remove(player.PlayerId);
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
                _players.Remove(player.PlayerId);
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
            if (_players.TryGetValue(uid, out GamePlayer player))
            {
                return player;
            }
            return null;
        }

        public void PlayerListPrint()
        {
            foreach (var player in _players)
            {
                Debug.Log(player.Value.Nickname);
            }
        }
    }
}
