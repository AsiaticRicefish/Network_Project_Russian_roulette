using DesignPattern;
using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemBoxTestTrigger : MonoBehaviourPun
{
    private Dictionary<string, ItemBoxManager> itemBoxDict = new();

    private void Start()
    {
        // 마스터가 아니면 리턴
        if (!PhotonNetwork.IsMasterClient) return;

        var allPlayers = PlayerManager.Instance.GetAllPlayers().Values;

        foreach (var player in allPlayers)
        {
            string playerId = player.PlayerId;

            if (ItemBoxSpawnerManager.Instance.TryGetItemBox(playerId, out var box))
            {
                itemBoxDict[playerId] = box;
                Debug.Log($"[TEST] {playerId} 상자 연결 완료");
            }
            else
            {
                Debug.LogWarning($"[TEST] {playerId} 상자 연결 실패");
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B) && PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[TEST] 마스터가 아이템 박스를 띄움");

            foreach (var kvp in itemBoxDict)
            {
                string playerId = kvp.Key;
                var boxManager = kvp.Value;

                if (boxManager == null)
                {
                    Debug.LogWarning($"[TEST] {playerId} 상자가 null입니다.");
                    continue;
                }

                var itemIds = GenerateRandomItemIds(boxManager.NewItemCount);
                photonView.RPC(nameof(RPC_ShowBoxWithItems), RpcTarget.All, itemIds, playerId);
            }

            // 나에게만 강제로 상자 띄우기
            if (Input.GetKeyDown(KeyCode.K))
            {
                string myId = PhotonNetwork.LocalPlayer.NickName;

                if (!ItemBoxSpawnerManager.Instance.TryGetItemBox(myId, out var myBox))
                {
                    Debug.LogWarning($"[TEST] [K] {myId} 상자 없음 → 생성 또는 연결 실패");
                    return;
                }

                var itemIds = GenerateRandomItemIds(myBox.NewItemCount);
                photonView.RPC(nameof(RPC_ShowBoxWithItems), RpcTarget.All, itemIds, myId);
                Debug.Log($"[TEST] [K] {myId}에게 강제 상자 노출 테스트");
            }
        }
    }

    private string[] GenerateRandomItemIds(int count)
    {
        var items = ItemDatabaseManager.Instance.GetRandomItems(count);
        return items.Select(item => item.itemId).ToArray();
    }

    [PunRPC]
    private void RPC_ShowBoxWithItems(string[] itemIds, string targetOwnerId)
    {
        if (!ItemBoxSpawnerManager.Instance.TryGetItemBox(targetOwnerId, out var targetBox))
        {
            Debug.LogWarning($"[TEST] {targetOwnerId}의 상자 못 찾음");
            return;
        }

        var itemDataList = itemIds
            .Select(id => ItemDatabaseManager.Instance.GetItemById(id))
            .Where(item => item != null)
            .ToList();

        targetBox.ShowBoxWithCustomItems(itemDataList);
        Debug.Log($"[TEST] {targetOwnerId}에게 아이템 {itemDataList.Count}개 지급 완료");
    }
}