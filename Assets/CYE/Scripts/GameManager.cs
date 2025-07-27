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
public class GameManager : Singleton<GameManager>
{
    // 참가자 순서 리스트 
    // => 다만 플레이어의 목록 자체는 PlayerManager에서 관리하므로 해당 값 또한 PlayerManager에서 관리해야할수도 있음
    private LinkedList<int> _turnOrder = new();
    private LinkedListNode<int> _currentTurnIndex;
    private int _currentRound;
    public int CurrentRound { get { return _currentRound; } private set { _currentRound = value; } }
    private int _totalRound;
    public int TotalRound { get { return _totalRound; } private set { _totalRound = value; } }

    // 각 게임 상태로 전환될때 실행되는 event 변수 
    // => 각 상태 전환시 필요한 동작을 개별적으로 
    public event Action OnGameStart;
    public event Action OnGameEnd;
    public event Action OnRoundStart;
    public event Action OnRoundEnd;
    public event Action OnTurnStart;
    public event Action OnTurnEnd;
    public event Action<bool> OnPaused;

    private void Awake() => SingletonInit();

    public void StartGame()
    {
        GameInit();

        OnGameStart?.Invoke();

        StartRound();
    }
    public void EndGame()
    {
        OnGameEnd?.Invoke();
        // 각 플레이어의 승패를 통해 결과 저장
    }

    public void StartRound()
    {
        RoundInit();

        OnRoundStart?.Invoke();

        StartTurn();
    }
    public void EndRound()
    {
        OnRoundEnd?.Invoke();

        if (_currentRound >= _totalRound)
        {
            EndGame();
            return;
        }

        StartRound();
    }

    public void StartTurn()
    {
        TurnInit();

        var currentPlayer = PlayerManager.Instance.GetPlayerByTurn(_currentTurnIndex.Value);
        if (currentPlayer != null && currentPlayer.IsCuffedThisTurn)
        {
            Debug.Log($"[턴 스킵] {currentPlayer.Nickname}는 수갑 상태 → 턴 스킵됨");
            currentPlayer.IsCuffedThisTurn = false; // 스킵 후 초기화
            EndTurn(); // 턴 바로 넘김
            return;
        }

        OnTurnStart?.Invoke();
    }
    public void EndTurn()
    {
        OnTurnEnd?.Invoke();
        
        // TO DO: 해당 조건을 확인할 주체를 고려해야함
        // if(플레이어들의 체력 == 0)
        //  EndRound()
        //  return

        _currentTurnIndex = _currentTurnIndex.Next;
        StartTurn();
    }

    public void PauseGame(bool changeState)
    {
        OnPaused?.Invoke(changeState);
    }

    private void GameInit()
    {
        _currentRound = 0;
    }
    private void RoundInit()
    {
        _currentRound++;
        _currentTurnIndex = _turnOrder.First;
    }
    private void TurnInit()
    {
        if (Manager.Gun.Magazine.Count == 0)
        { 
            Manager.Gun.Reload();
        }
    }
}
