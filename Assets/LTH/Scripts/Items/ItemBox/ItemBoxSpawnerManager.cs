using DesignPattern;
using Managers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    [Header("Prefab & Spawn Points")]
    [SerializeField] private GameObject itemBoxPrefab;         // 상자 프리팹
    [SerializeField] private Transform[] spawnPoints;          // 상자 배치 위치

    private Dictionary<string, ItemBoxManager> itemBoxDict = new();

    private void Awake() => SingletonInit();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Manager.Game != null);

        Manager.Game.OnTurnEnd += HandleTurnEnd;
        Manager.Game.OnGameStart += HandleGameStart;
    }

    private void OnDestroy()
    {
        if (Manager.Game != null)
        {
            Manager.Game.OnTurnEnd -= HandleTurnEnd;
            Manager.Game.OnGameStart -= HandleGameStart;
        }
    }

    /// <summary>
    /// 게임 시작 시 상자 미리 생성해서 비활성화 상태로 준비
    /// </summary>
    private void HandleGameStart()
    {
        Debug.Log("[Spawner] 게임 시작됨 → ItemBox 사전 생성");

        var allPlayers = PlayerManager.Instance.GetAllPlayers().Values;
        int i = 0;

        foreach (var player in allPlayers)
        {
            if (i >= spawnPoints.Length)
            {
                Debug.LogWarning("[Spawner] 스폰 포인트 부족! 추가 필요");
                continue;
            }

            string playerId = player.PlayerId;
            var desk = DeskUIManager.Instance.GetDeskUI(playerId);

            GameObject boxObj = Instantiate(itemBoxPrefab, spawnPoints[i].position, spawnPoints[i].rotation);
            var box = boxObj.GetComponent<ItemBoxManager>();
            box.Init(playerId, desk);
            box.gameObject.SetActive(false);

            itemBoxDict[playerId] = box;
            i++;
            Debug.Log($"[Spawner] {playerId}의 상자 생성 완료");
        }
    }

    /// <summary>
    /// 턴 종료 시 각 플레이어의 상자 활성화
    /// </summary>
    private void HandleTurnEnd()
    {
        Debug.Log("[Spawner] 턴 종료 → 아이템 박스 등장");

        foreach (var (playerId, box) in itemBoxDict)
        {
            if (box == null) continue;

            box.ShowBox();

            var sync = FindObjectsOfType<ItemSync>()
                        .FirstOrDefault(s => s.photonView.IsMine);
            if (sync != null)
            {
                sync.Init(playerId, box);
            }
            else
            {
                Debug.LogWarning($"[Spawner] {playerId}의 ItemSync 없음");
            }
        }
    }

    /// <summary>
    /// 외부에서 ItemBox 접근 시 사용
    /// </summary>
    public bool TryGetItemBox(string playerId, out ItemBoxManager itemBox)
    {
        return itemBoxDict.TryGetValue(playerId, out itemBox);
    }
}