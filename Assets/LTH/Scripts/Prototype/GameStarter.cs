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
            StartGame();
        }
    }

    private void RegisterTurnOrder()
    {
        var players = PlayerManager.Instance.GetAllPlayers();
        var turnOrder = new LinkedList<string>();

        foreach (var kvp in players)
        {
            string id = kvp.Key;
            turnOrder.AddLast(id);
        }

        var field = typeof(InGameManager).GetField("_turnOrder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field?.SetValue(InGameManager.Instance, turnOrder);
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
