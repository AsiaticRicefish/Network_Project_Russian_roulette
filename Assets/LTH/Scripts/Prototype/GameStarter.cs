using Managers;
using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private bool started = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            RegisterTurnOrder();
            Debug.Log("[GameStarter] 마스터 클라이언트 → InGameManager.StartGame 호출");
            StartGame();
        }
        else
        {
            Debug.Log("[GameStarter] 일반 클라이언트, 게임 시작 대기");
        }
    }

    private void RegisterTurnOrder()
    {
        var players = PlayerManager.Instance.GetAllPlayers(); // UID 기준
        var turnOrder = new LinkedList<string>();

        foreach (var kvp in players)
        {
            string id = kvp.Key; // Firebase UID or Photon NickName
            Debug.Log($"[GameStarter] 턴 순서 등록: {id}");
            turnOrder.AddLast(id);
        }

        // InGameManager._turnOrder 에 리플렉션으로 접근
        var field = typeof(InGameManager).GetField("_turnOrder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(InGameManager.Instance, turnOrder);

        Debug.Log("[GameStarter] _turnOrder 강제 등록 완료");
    }

    private void StartGame()
    {
        if (started) return;
        started = true;

        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogWarning("[GameStarter] InGameManager 인스턴스를 찾을 수 없습니다!");
        }
    }
}
