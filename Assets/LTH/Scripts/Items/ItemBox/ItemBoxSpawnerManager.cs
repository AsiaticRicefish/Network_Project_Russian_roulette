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
/// </summary>

public class ItemBoxSpawnerManager : Singleton<ItemBoxSpawnerManager>
{
    [Header("Prefab & Spawn Points")]
    [SerializeField] private Transform[] spawnPoints;

    private Dictionary<string, ItemBoxManager> itemBoxDict = new();

    public bool IsInitialized { get; private set; } = false;

    private void Awake() => SingletonInit();

    private IEnumerator Start()
    {
        yield return new WaitUntil(() => Manager.Game != null);

        Manager.Game.OnTurnEnd += HandleTurnEnd;
        Manager.Game.OnGameStart += () => IsInitialized = true;
    }

    /// <summary>
    ///  Try - catch 구문을 사용하여 예외 처리 (Manager.Game이 null일 수 있는 상황인데도 접근되는 경우)
    /// </summary>
    private void OnDestroy()
    {
        try // 여기 안의 코드에서 오류가 나더라도 프로그램이 멈추지 않고 catch로 넘어감
        {
            if (Manager.Game != null)
            {
                Manager.Game.OnTurnEnd -= HandleTurnEnd;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning($"[ItemBoxSpawnerManager] OnDestroy 예외 발생: {e.Message}");
        }
    }

    /// <summary>
    /// 외부에서 자신이 생성한 ItemBox를 등록
    /// </summary>
    public void RegisterItemBox(string nickname, ItemBoxManager box)
    {
        if (!itemBoxDict.ContainsKey(nickname))
        {
            itemBoxDict[nickname] = box;
            Debug.Log($"[ItemBoxSpawnerManager] 등록됨 → {nickname}, ViewID: {box.GetComponent<PhotonView>()?.ViewID}");
        }
        else
        {
            Debug.LogWarning($"[ItemBoxSpawnerManager] 이미 등록된 플레이어: {nickname}");
        }
    }

    /// <summary>
    /// 턴 종료 시 각 상자 등장만 처리
    /// </summary>
    private void HandleTurnEnd()
    {
        Debug.Log("[ItemBoxSpawnerManager] 턴 종료 → 모든 상자 등장");

        foreach (var (_, box) in itemBoxDict)
        {
            box?.ShowBox();
        }
    }

    /// <summary>
    /// 모든 플레이어의 상자를 수동으로 등장시키는 함수 (예: 탄이 다 떨어졌을 때 호출)
    /// </summary>
    public void ShowAllBoxes()
    {
        Debug.Log("[Spawner] 수동 호출: ShowAllBoxes() → 상자 등장");

        foreach (var (_, box) in itemBoxDict)
        {
            box?.ShowBox();
        }
    }

    /// <summary>
    /// 외부에서 ItemBox 접근 시 사용
    /// </summary>
    public bool TryGetItemBox(string playerId, out ItemBoxManager itemBox)
    {
        return itemBoxDict.TryGetValue(playerId, out itemBox);
    }

    public void Clear()
    {
        itemBoxDict.Clear();
        IsInitialized = false;
    }
}