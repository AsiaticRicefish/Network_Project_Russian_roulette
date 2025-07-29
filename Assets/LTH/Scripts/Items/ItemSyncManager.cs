using DesignPattern;
using LTH;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Rendering; // 임시 Playerm ItemData 정의용 네임스페이스
using UnityEngine;


// using Photon.Pun;
// using Photon.Realtime;
// using ExitGames.Client.Photon;

/// <summary>
/// 아이템 동기화 관리 클래스
/// </summary>

public class ItemSyncManager : Singleton<ItemSyncManager>
{
    // 각 플레이어별 동기화된 아이템 목록 저장
    private Dictionary<string, List<ItemData>> syncedItems = new(); // key: player.NickName

    private void Awake()
    {
        SingletonInit();
    }

    /// <summary>
    /// 아이템을 생성하고 네트워크로 전송하는 역할  (로컬에서 생성하고 네트워크 전송 예정)
    /// </summary>
    public void GenerateAndSync(string playerId)
    {
        var generatedItems = new List<ItemData>
        {
            ItemDatabaseManager.Instance.GetItemById("cigarette"),
            ItemDatabaseManager.Instance.GetItemById("cuffs"),
            ItemDatabaseManager.Instance.GetItemById("magnifyingGlass"),
            ItemDatabaseManager.Instance.GetItemById("saw"),
            //new ItemData(ItemType.Cigarette, "담배", "플레이어 체력 1 회복합니다."),
            //new ItemData(ItemType.Cuffs, "수갑", "상대 플레이어 턴 1회 건너뜁니다"),
            //new ItemData(ItemType.MagnifyingGlass, "돋보기", "실탄인지 공포탄인지 구분합니다."),
            //new ItemData(ItemType.Saw, "톱", "총 2배로 데미지 증가합니다.")
        };

        syncedItems[playerId] = generatedItems;

        Debug.Log($"[ItemSyncManager] 아이템 생성 및 동기화됨: {playerId}");
    }

    /// <summary>
    /// 다른 플레이어가 보낸 아이템을 받아서 저장
    /// </summary>
    public void OnSyncReceived(string playerId, List<ItemData> items)
    {
        // 네트워크를 통해 받은 아이템 데이터를 해당 플레이어에 저장
        syncedItems[playerId] = items;
        
        Debug.Log($"[ItemSyncManager] 아이템 동기화 수신됨: {playerId}");
    }

    /// <summary>
    /// 특정 플레이어가 현재 어떤 아이템을 동기화받았는지 확인
    /// </summary>
    public List<ItemData> GetSyncedItems(string playerId)
    {
        // Dictionary에 존재하는 경우 해당 아이템 목록 반환
        if (syncedItems.TryGetValue(playerId, out var items)) return items;

        // 없으면 빈 리스트 반환 (null 방지)
        return new List<ItemData>();
    }
}