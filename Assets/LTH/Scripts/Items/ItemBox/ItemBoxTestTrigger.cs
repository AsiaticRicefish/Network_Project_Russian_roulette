using DesignPattern;
using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemBoxTestTrigger : MonoBehaviourPun
{
    [Header("플레이어별 상자 매핑")]
    [SerializeField] private List<ItemBoxManager> itemBoxManagers;

    private void Update()
    {
        // B 키 입력 시, 마스터 클라이언트가 각 플레이어에게 보상 아이템 할당
        if (Input.GetKeyDown(KeyCode.B) && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[TEST] 마스터가 아이템 박스를 띄움");

            foreach (var boxManager in itemBoxManagers)
            {
                if (boxManager == null || string.IsNullOrEmpty(boxManager.OwnerId))
                {
                    Debug.LogWarning("[TEST] BoxManager가 null이거나 OwnerId가 비어 있음 → 스킵");
                    continue;
                }

                // 개별 상자에 지정된 보상 개수 기반으로 아이템 생성
                var itemIds = GenerateRandomItemIds(boxManager.NewItemCount);
                photonView.RPC(nameof(RPC_ShowBoxWithItems), RpcTarget.All, itemIds, boxManager.OwnerId);
            }
        }
    }

    /// <summary>
    /// 주어진 개수만큼 랜덤 아이템 ID 리스트 생성
    /// </summary>
    private string[] GenerateRandomItemIds(int count)
    {
        var items = ItemDatabaseManager.Instance.GetRandomItems(count);
        var ids = new List<string>();

        foreach (var item in items)
        {
            if (item != null)
                ids.Add(item.itemId);
        }

        return ids.ToArray();
    }

    /// <summary>
    /// 각 클라이언트에서 자신에게 해당하는 상자에 아이템 지급
    /// </summary>
    [PunRPC]
    private void RPC_ShowBoxWithItems(string[] itemIds, string targetOwnerId)
    {
        foreach (var boxManager in itemBoxManagers)
        {
            if (boxManager == null || boxManager.OwnerId != targetOwnerId)
                continue;

            var itemDataList = new List<ItemData>();
            foreach (var id in itemIds)
            {
                var item = ItemDatabaseManager.Instance.GetItemById(id);
                if (item != null)
                    itemDataList.Add(item);
            }

            boxManager.ShowBoxWithCustomItems(itemDataList);
            Debug.Log($"[TEST] {targetOwnerId}에게 아이템 {itemDataList.Count}개 지급 완료");
        }
    }
}