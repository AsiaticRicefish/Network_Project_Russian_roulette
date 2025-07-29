using DesignPattern;
using Managers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 아이템 박스 매니저를 관리하고, 턴 종료 시 아이템 박스를 소환하는 매니저
/// Manager.Game.OnTurnEnd 이벤트에 등록되어 턴 종료 시 아이템 박스를 소환합니다.
/// 
/// Manager.cs에 접근 프로퍼티와 등록 코드 추가해야 함
/// public static ItemBoxSpawnerManager ItemBoxSpawner => ItemBoxSpawnerManager.Instance;
/// manager.AddComponent<ItemBoxSpawnerManager>();
/// 
/// </summary>

public class ItemBoxSpawnerManager : Singleton<ItemBoxSpawnerManager>
{
    [SerializeField] private List<ItemBoxManager> itemBoxManagers;

    private void Awake() => SingletonInit();

    private void Start()
    {
        Manager.Game.OnTurnEnd += HandleTurnEnd; // 턴 끝날 때 박스 생성
        Manager.Game.OnGameStart += HandleGameStart;
    }

    private void OnDestroy()
    {
        if (Manager.Game != null)
        {
            Manager.Game.OnTurnEnd -= HandleTurnEnd;
        }
    }

    private void HandleGameStart()
    {
        Debug.Log("[Spawner] 게임 시작됨 → 상자 초기화 또는 사전 준비 가능");
    }

    private void HandleTurnEnd()
    {
        Debug.Log("[Spawner] 턴 종료 → 아이템 박스 소환");

        var allPlayers = PlayerManager.Instance.GetAllPlayers().Values;

        int i = 0;
        foreach (var player in allPlayers)
        {
            if (i >= itemBoxManagers.Count) break;

            string playerId = player.PlayerId;
            var desk = DeskUIManager.Instance.GetDeskUI(playerId);

            if (desk == null) continue;

            itemBoxManagers[i].Init(playerId, desk);
            itemBoxManagers[i].ShowBox();

            i++;
        }
    }
}