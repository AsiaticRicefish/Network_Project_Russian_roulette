using DesignPattern;
using LTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestCopyItemSync : Singleton<TestCopyItemSync>
{
    private Dictionary<string, List<ItemData>> syncedItems = new();

    public void GenerateAndSync(string id)
    {
        // ToDo: 실제 게임 로직에 맞는 아이템 생성 로직으로 교체 필요
        var generatedItems = new List<ItemData>
        {
            new ItemData(ItemType.Cigarette, "담배", "플레이어 체력 1 회복합니다."),
            new ItemData(ItemType.Cellphone, "휴대폰", "장전된 탄환에 대한 미래 예지가 가능합니다.")
        };

        // 로컬에 생성된 아이템을 저장
        syncedItems[id] = generatedItems;

        // ToDo: 이 아이템 리스트를 네트워크로 다른 클라이언트에게 전송해야 함
        // RPC로 전송 시 SerializeItemList() 같은 방식으로 직렬화 필요
        //Debug.Log($"[ItemSyncManager] 아이템 생성 및 동기화됨: {player.FirebaseUID}"); ;
    }

    /// <summary>
    /// 다른 플레이어가 보낸 아이템을 받아서 저장
    /// </summary>
    public void OnSyncReceived(string id, List<ItemData> items)
    {
        // 네트워크를 통해 받은 아이템 데이터를 해당 플레이어에 저장
        syncedItems[id] = items;
        // ToDo: 이 메서드는 Photon의 OnEvent 또는 RPC 수신 시 호출되어야 함
       // Debug.Log($"[ItemSyncManager] 아이템 동기화 수신됨: {player.FirebaseUID}");
    }

    /// <summary>
    /// 특정 플레이어가 현재 어떤 아이템을 동기화받았는지 확인
    /// </summary>
    public List<ItemData> GetSyncedItems(string id)
    {
        // Dictionary에 존재하는 경우 해당 아이템 목록 반환
        if (syncedItems.TryGetValue(id, out var items)) return items;

        // 없으면 빈 리스트 반환 (null 방지)
        return new List<ItemData>();
    }
}
