using DesignPattern;
using LTH;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

/// <summary>
/// 각 플레이어의 아이템 동기화 상태를 관리하는 싱글톤 매니저.
/// 기준 식별자는 Photon 닉네임.
/// </summary>

public class ItemSyncManager : Singleton<ItemSyncManager>
{
    // 각 플레이어 닉네임 → 아이템 리스트 매핑
    private Dictionary<string, List<ItemData>> syncedItems = new();

    private void Awake()
    {
        SingletonInit();
    }

    /// <summary>
    /// 네트워크로부터 아이템 동기화 수신 (ItemSync에서 호출)
    /// </summary>
    public void OnSyncReceived(string nickname, List<ItemData> items)
    {
        if (items == null) return;

        syncedItems[nickname] = new List<ItemData>(items);
        foreach(var i in items)
        Debug.Log($"[ItemSyncManager] 아이템 동기화 수신 완료: {nickname}, 개수: {items.Count}");
    }

    /// <summary>
    /// 특정 닉네임 기준으로 동기화된 아이템 리스트 반환
    /// </summary>
    public List<ItemData> GetSyncedItems(string nickname)
    {
        if (syncedItems.TryGetValue(nickname, out var items)) return items;

        return new List<ItemData>();
    }

    public void RegisterSyncedItem(string nickname, ItemData item)
    {
        if (!syncedItems.ContainsKey(nickname))
            syncedItems[nickname] = new List<ItemData>();

        // 이미 같은 unique ID가 등록되어 있다면 중복 방지
        if (!syncedItems[nickname].Any(i => i.uniqueInstanceId == item.uniqueInstanceId))
        {
            syncedItems[nickname].Add(item);
        }
    }

    /// <summary>
    /// 존재 여부를 반환하는 TryGet 오버로드
    /// </summary>
    public bool TryGetSyncedItems(string nickname, out List<ItemData> items)
    {
        return syncedItems.TryGetValue(nickname, out items);
    }
}