using DesignPattern;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun; 
using Photon.Realtime;
using ExitGames.Client.Photon;
using System;

//TODO - 만약 게임 플레이 중간에 플레이어가 나가면 해당 유저의 정보를 삭제해야하는 부분이 필요 
//인게임 플레이어 매니저를 만드는게 빠를것같다.
public class PlayerManager : Singleton<PlayerManager>
{
    //리스트가 아닌 Dictionary로 관리  Key - FirebaseUID , Value - GamePlayer
    //리스트를 딱한번 공유해야하는 상황 -> 모든 유저가 다 등록이 되었을 때 모든 유저가 해당 데이터를 들고 있도록 해야한다.
    [SerializeField] public List<GamePlayer> _playerList;
    [SerializeField] private Dictionary<string, GamePlayer> _players;
    

    //플레이어 생성전 플레이어의 데이터를 firebase들고와서 key:firebaseId Value:PlayerData로 저장해놓는다.
    //지금 당장 필요할 것 같지는 않다.
    [SerializeField] public Dictionary<string, PlayerData> _playerData;
    [SerializeField] public List<PlayerData> _playerDataList;

    public Dictionary<string, GamePlayer> GetAllPlayers() => _players;

    public Action OnAddPlayer;

    private void Awake()
    {
        SingletonInit();
        Init();
    }

    private void OnEnable()
    {
        InGameManager.Instance.OnGameStart += AllGamePlayerAdd;
    }

    // TODO - 해제를 해주는 곳이 애매하다.
    private void OnDisable()
    {
        _players?.Clear();
        _playerList?.Clear();
        //InGameManager.Instance.OnGameStart -= AllGamePlayerAdd; 
    }

    //게임 스타트시 사용해야하는부분
    public void AllGamePlayerAdd()
    {
        GamePlayer[] players = FindObjectsOfType<GamePlayer>();
        Debug.Log($"마스터 클라이언트: 씬에서 {players.Length} 개의 GamePlayer 객체를 찾았습니다.");

        foreach (GamePlayer player in players) 
        {
            RegisterPlayer(player);
            _playerList.Add(player);
            Debug.Log($"등록된 플레이어 : {player.Nickname}, 플레이어 ID : {player.PlayerId}");
        }

        if (_players.Count != PhotonNetwork.CurrentRoom.PlayerCount)
        {
            Debug.Log($"dic플레이어 추가 오류! 현재 방안에 존재하는 플레이어 수 : {PhotonNetwork.CurrentRoom.PlayerCount},딕셔너리에 저장된 플레이어 수 : {_players.Count}");           
        }
    }

    private void Init()
    {
        _players = new Dictionary<string, GamePlayer>();
        _playerList = new List<GamePlayer>();
        //_playerDataList = new List<PlayerData>();
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

    #region 플레이어 Dictionary 관련 함수 - 추가,제거,검색(ID기반,닉네임기반)
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
            
            OnAddPlayer?.Invoke();
        }
        else
        {
            Debug.LogWarning($"Player ID : {player.PlayerId} 에 해당되는 유저가 이미 존재 합니다.");
        }
    }

    public void RemovePlayer(GamePlayer player)
    {
        if (RemovePlayerByUID(player.PlayerId))
        {
            _players.Remove(player.PlayerId);
        }
        else
        {
            Debug.LogWarning($"Player ID : {player.PlayerId}에 해당되는 유저가 존재하지 않습니다.");
        }
    }

    //Player 고유값인 uid로 리스트에서 삭제하기 
    private bool RemovePlayerByUID(string uid)
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

    public GamePlayer FindPlayerByNickname(string nickname)
    {
        foreach (var player in _players.Values)
        {
            if (player.Nickname == nickname)
            {
                return player;
            }
        }

        Debug.LogWarning($"[PlayerManager] 닉네임 {nickname} 을 가진 플레이어를 찾지 못했습니다.");
        return null;
    }

    //Dictionary에 추가되어 있는 유저들 확인(디버깅용)
    public void PlayerListPrint()
    {
        foreach (var player in _players)
        {
            Debug.Log(player.Value.Nickname);
        }
    }
    #endregion

    /*
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
    }*/
}
