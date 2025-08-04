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
        yield return new WaitUntil(() => PhotonNetwork.IsConnected && PhotonNetwork.InRoom);

        Manager.Game.OnGameStart += () => IsInitialized = true;
    }

    /// <summary>
    /// 외부에서 자신이 생성한 ItemBox를 등록
    /// </summary>
    public void RegisterItemBox(string nickname, ItemBoxManager box)
    {
        if (!itemBoxDict.ContainsKey(nickname))
        {
            itemBoxDict[nickname] = box;
        }
    }

    /// <summary>
    /// 모든 플레이어의 상자를 수동으로 등장시키는 함수 (예: 탄이 다 떨어졌을 때 호출)
    /// </summary>
    public void ShowAllBoxes()
    {
        foreach (var (_, box) in itemBoxDict)
        {
            box?.ShowBox();
        }
    }

    /// <summary>
    /// 외부에서 ItemBox 접근 시 사용
    /// </summary>
    public bool TryGetItemBox(string nickname, out ItemBoxManager itemBox)
    {
        return itemBoxDict.TryGetValue(nickname, out itemBox);
    }

    public void Clear()
    {
        itemBoxDict.Clear();
        IsInitialized = false;
    }

    /// <summary>
    /// 각 플레이어가 자신의 인덱스에 맞는 스폰 포인트를 가져오기 위한 유틸리티
    /// </summary>
    public Transform GetSpawnPoint(int index)
    {
        if (index < 0 || index >= spawnPoints.Length) return null;
        return spawnPoints[index];
    }
}