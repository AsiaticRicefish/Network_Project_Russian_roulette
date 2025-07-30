using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using ExitGames.Client.Photon;

//인게임 플레이어 매니저를 만드는게 빠를것같다.
public class PlayerManager : Singleton<PlayerManager>
{
    //리스트가 아닌 Dictionary로 관리  Key - FirebaseUID Value - PlayData
    public Dictionary<string, PlayerData> _playerData;
    public List<GamePlayer> _playerList;

    //리스트를 딱한번 공유해야하는 상황 -> 모든 유저가 다 등록이 되었을 때 모든 유저가 해당 데이터를 들고 있도록 해야한다.
    public List<PlayerData> _playerDataList;
    private Dictionary<string, GamePlayer> _players;  


    public Dictionary<string, GamePlayer> GetAllPlayers() => _players;

    private void Awake()
    {
        SingletonInit();
        Init();
    }

    //게임 스타트시 추가
    /*public bool AllGamePlayerAdd()
    {
        if (!PhotonNetwork.IsMasterClient) return false;

        GamePlayer[] players = FindObjectsOfType<GamePlayer>();
        Debug.Log($"마스터 클라이언트: 씬에서 {players.Length} 개의 GamePlayer 객체를 찾았습니다.");

        foreach (GamePlayer player in players) 
        {
            RegisterPlayer(player);
            Debug.Log($"등록된 플레이어 : {player.Nickname}, 플레이어 ID : {player.PlayerId}");
        }

        if (_players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log($"dic플레이어 추가 오류! 현재 방안에 존재하는 플레이어 수 : {PhotonNetwork.CurrentRoom.PlayerCount},딕셔너리에 저장된 플레이어 수 : {_players.Count}");           
            return false;
        }

        //딕셔너리에 다 추가 된 상황이면 해당 Dictionary를 게임매니저 List에게 넣게 해줘야한다.
        //InGameManager.Instance._playerList.Add(players);
        return true;
    }*/

    private void Init()
    {
        _players = new Dictionary<string, GamePlayer>();
        _playerList = new List<GamePlayer>();
        _playerDataList = new List<PlayerData>();
    }

    //안씀
    public GamePlayer CreateGamePlayer(Player photonPlayer, PlayerData playerData)
    {
        string uid = photonPlayer.CustomProperties["playerId"]?.ToString() ?? "unknown";        //등록
        string nickname = photonPlayer.NickName;                                                //닉네임으로 설정

        GamePlayer gamePlayer = new GamePlayer();
        //gamePlayer.Initialize(playerData);
        RegisterPlayer(gamePlayer);
        return gamePlayer;
    }


    public void RegisterPlayer(GamePlayer player)
    {
        if (string.IsNullOrEmpty(player.PlayerId))
        {
            Debug.LogError("null값 등록시도.");
            return;
        }

        if (!_players.ContainsKey(player.PlayerId))
        {
            _players.Add(player.PlayerId, player);
            Debug.Log($"Player added! Player FirebaseUID Number : {player.PlayerId}, Player NickName : {player.Nickname}. Current players: {_players.Count}");
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

    public PlayerData GetFindPlayerDataFromID(string id)
    {
        foreach(var playerData in _playerDataList) 
        {
            if (playerData.playerId == id)
                return playerData;
        }

        Debug.Log($"해당ID : {id} 와 일치 하는 ID를 가진 유저가 존재 하지 않습니다.");
        return null;
    }

    public void PrintPlayerList()
    {
        string str = "";
        foreach(var p in _playerList)
        {
            str += p.Nickname + " ";
        }
        Debug.Log(str);
    }
}
