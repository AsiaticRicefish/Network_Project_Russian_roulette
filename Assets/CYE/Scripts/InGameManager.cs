using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DesignPattern;
using UnityEngine.Events;
using System;
using Managers;

/// <summary>
/// 게임 플레이의 흐름을 관리하는 클래스.
/// </summary>
public class InGameManager : Singleton<InGameManager>
{
    private const int MAX_ROUND = 3;

    // TO DO: 금일 머지 작업 후 _playerList 사용 부분을 Manager.Player.players로 변경해야 함
    private List<GamePlayer> _playerList = new();
    private LinkedList<string> _turnOrder = new();
    private LinkedListNode<string> _currentTurn;
    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } private set { _currentRound = value; } }
    private int _totalRound;
    public int TotalRound { get { return _totalRound; } private set { _totalRound = value; } }

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

    private void Awake() => Init();
    private void Init()
    {
        SingletonInit();
        _totalRound = MAX_ROUND;
    }

    /// <summary>
    /// 게임 시작시 호출되는 함수
    /// </summary>
    public void StartGame()
    {
        Debug.Log("StartGame");
        GameInit();

        OnGameStart?.Invoke();

        StartRound();
    }
    /// <summary>
    /// 게임 종료시 호출되는 함수
    /// </summary>
    public void EndGame()
    {
        Debug.Log("EndGame");
        OnGameEnd?.Invoke();
        // 각 플레이어의 승패를 통해 결과 저장
    }
    /// <summary>
    /// 한 라운드 시작시 호출되는 함수
    /// </summary>
    public void StartRound()
    {
        Debug.Log("StartRound");
        RoundInit();

        OnRoundStart?.Invoke();

        StartTurn();
    }
    /// <summary>
    /// 한 라운드 종료시 호출되는 함수
    /// </summary>
    public void EndRound()
    {
        Debug.Log("EndRound");
        OnRoundEnd?.Invoke();

        if (_currentRound >= _totalRound)
        {
            EndGame();
            return;
        }

        StartRound();
    }
    /// <summary>
    /// 한 턴 시작시 호출되는 함수
    /// </summary>
    public void StartTurn()
    {
        Debug.Log("StartTurn");
        TurnInit();

        OnTurnStart?.Invoke();
    }
    /// <summary>
    /// 한 턴 종료시 호출되는 함수
    /// </summary>
    public void EndTurn()
    {
        Debug.Log("EndTurn");
        OnTurnEnd?.Invoke();

        foreach (GamePlayer player in _playerList)
        {
            if (player.CurrentHp == 0)
            {
                EndGame();
                return;
            }
        }

        _currentTurn = _currentTurn.Next;
        StartTurn();
    }
    /// <summary>
    /// 일시 정지시 호출되는 함수
    /// </summary>
    /// <param name="isPause">일시 정지 여부(true=일시 정지/false=일시 정지 해제)</param>
    public void PauseGame(bool isPause)
    {
        Debug.Log("PauseGame");
        OnPaused?.Invoke(isPause);
    }

    private void GameInit()
    {
        _currentRound = 0;
    }
    private void RoundInit()
    {
        _currentRound++;
        _currentTurn = _turnOrder.First;
    }
    private void TurnInit()
    {
        // 턴 시작시 뭐... 실행할거 암거나 부르기
    }
}
