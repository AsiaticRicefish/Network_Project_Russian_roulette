using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPattern;
using UnityEngine.Events;
using System;
using Managers;
using Photon.Pun;

/// <summary>
/// 게임 플레이의 흐름을 관리하는 클래스.
/// </summary>
public class InGameManager : Singleton<InGameManager>
{
    #region >> Internal Class
    private class PlayerPointPair
    {
        private string _playerId;
        public string PlayerId { get { return _playerId; } }
        private int _winCount;
        public int WinCount { get { return _winCount; } }
        private int _loseCount;
        public int LoseCount { get { return _loseCount; } }
        public PlayerPointPair(string playerId)
        {
            _playerId = playerId;
            _winCount = 0;
            _loseCount = 0;
        }
        public void IncreaseWinCount()
        {
            _winCount++;
        }
        public void IncreaseLoseCount()
        {
            _loseCount++;
        }
    }
    #endregion

    #region >> Constants
    private const int MAX_ROUND = 3;
    #endregion

    #region  >> Variables
    private LinkedList<string> _turnOrder = new();
    private LinkedListNode<string> _currentTurn;
    public string CurrentTurn { get { return _currentTurn.Value; } }
    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } private set { _currentRound = value; } }
    private int _totalRound;
    public int TotalRound { get { return _totalRound; } private set { _totalRound = value; } }
    private List<PlayerPointPair> _playerPointPair = new();
    #endregion

    #region  >> Events
    // 각 게임 상태로 전환될때 실행되는 event 변수 
    // => 각 상태 전환시 필요한 동작을 개별적으로 등록
    // TO DO: Invoke를 서버(마스터 클라이언트)에서 실행, 플레이어는 해당 결과를 받기만 해야함.
    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnTurnStart;
    public event Action OnTurnEnd;
    public event Action<bool> OnPaused;

    public event Action OnRoundCountChange;
    public event Action OnTurnChange;
    #endregion

    #region >> Unity Message Function
    private void Awake() => Init();
    private void Init()
    {
        SingletonInit();
        _totalRound = MAX_ROUND;
    }
    #endregion

    #region >> Public Function
    /// <summary>
    /// 게임 시작시 호출되는 함수
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame");

        // 게임 시작시 초기화 진행
        GameInit();

        RegisterPlayerDeathEvents();

        // 게임 시작시 이벤트 실행
        OnGameStart?.Invoke();

        // 라운드 시작
        StartRound();
    }
    
    /// <summary>
    /// 게임 종료시 호출되는 함수
    /// </summary>
    public void EndGame()
    {
        Debug.Log("EndGame");

        // TO DO: 각 플레이어의 승패를 통해 결과 저장

        // 게임 종료시 이벤트 실행
        OnGameEnd?.Invoke();
    }

    /// <summary>
    /// 한 라운드 시작시 호출되는 함수
    /// </summary>
    public void StartRound()
    {
        Debug.Log("StartRound");

        // 라운드 시작시 초기화 진행
        RoundInit();

        // 라운드 시작시 이벤트 실행
        OnRoundStart?.Invoke();

        // 턴 시작
        StartTurn();
    }

    /// <summary>
    /// 한 라운드 종료시 호출되는 함수
    /// </summary>
    public void EndRound()
    {
        Debug.Log("EndRound");

        // 승패 카운트 저장
        SpecifyWinLoseCount();

        // 라운드 종료시 이벤트 실행
        OnRoundEnd?.Invoke();

        // 만약 현재 라운드가 마지막 라운드일 경우 게임을 종료함.
        if (_currentRound >= _totalRound)
        {
            EndGame();
            return;
        }

        // 라운드 시작
        StartRound();
    }

    /// <summary>
    /// 한 턴 시작시 호출되는 함수
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("StartTurn");

        // 턴 시작시 초기화 진행
        TurnInit();

        // 턴 시작시 이벤트 실행
        OnTurnStart?.Invoke();
    }

    /// <summary>
    /// 한 턴 종료시 호출되는 함수
    /// </summary>
    public void EndTurn()
    {
        Debug.Log("EndTurn");

        // 턴 종료시 이벤트 실행
        OnTurnEnd?.Invoke();

        // 라운드 종료 확인
        if (CheckRoundEnd())
        {
            EndRound();
            return;
        }

        // 현재 턴을 다음으로 넘긴다.
        _currentTurn = _currentTurn.Next ?? _turnOrder.First;
        // 턴 시작
        StartTurn();
    }

    /// <summary>
    /// 일시 정지시 호출되는 함수
    /// </summary>
    /// <param name="isPause">일시 정지 여부(true=일시 정지/false=일시 정지 해제)</param>
    public void PauseGame(bool isPause)
    {
        Debug.Log("PauseGame");

        // 일시 정지시 이벤트 실행
        OnPaused?.Invoke(isPause);
    }
    #endregion

    #region >> Private Function
    private void GameInit()
    {
        // 현재 라운드를 초기화한다.
        _currentRound = 0;

        // 플레이어 승패 수 관리용 변수(List)를 초기화한다.
        _playerPointPair = new();
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            string playerId = p.NickName;
            _playerPointPair.Add(new PlayerPointPair(playerId));
            _turnOrder.AddLast(playerId);
        }
    }
    private void RoundInit()
    {
        // 현재 라운드 카운트 증가시킨다.
        _currentRound++;
        OnRoundCountChange?.Invoke();

        // 현재 턴을 턴 순서 맨 처음 사람으로 지정한다.
        _currentTurn = _turnOrder.First;
    }

    private void TurnInit()
    {
        OnTurnChange?.Invoke();
    }

    private bool CheckRoundEnd()
    {
        // CurrentHp가 0이 아닌 플레이어, 즉 생존자의 수를 확인한다.
        int survivorCount = 0;
        foreach (KeyValuePair<string, GamePlayer> item in Manager.PlayerManager.GetAllPlayers())
        {
            if (item.Value.CurrentHp > 0)
            {
                survivorCount++;
            }
        }

        // 생존자가 한 명일 경우 현재 라운드를 종료하도록 한다.
        return survivorCount == 1;
    }

    private void SpecifyWinLoseCount()
    {
        // 플레이어 리스트를 가져온다
        foreach (KeyValuePair<string, GamePlayer> item in Manager.PlayerManager.GetAllPlayers())
        {
            // hp가 0이 아닐 경우(호출 시점에서 해당 조건에 부합하는 플레이어는 한 명임을 확인했다),
            if (item.Value.CurrentHp > 0)
            {
                // 해당 플레이어의 WinCount(승 수)를 증가시킨다.
                _playerPointPair.Find(x => x.PlayerId == item.Value.PlayerId).IncreaseWinCount();
            }
            // hp가 0이하일 경우,
            else
            {
                // 해당 플레이어의 LoseCount(패 수)를 증가시킨다.
                _playerPointPair.Find(x => x.PlayerId == item.Value.PlayerId).IncreaseLoseCount();
            }
        }
    }

    private void RegisterPlayerDeathEvents()
    {
        foreach (var player in Manager.PlayerManager.GetAllPlayers().Values)
        {
            player.OnPlayerDied += OnPlayerDiedHandler;
        }
    }

    private void OnPlayerDiedHandler(GamePlayer deadPlayer)
    {
        Debug.Log($"[InGameManager] 사망 감지: {deadPlayer.Nickname}");
        if (CheckRoundEnd()) EndRound();
    }
    #endregion
}
