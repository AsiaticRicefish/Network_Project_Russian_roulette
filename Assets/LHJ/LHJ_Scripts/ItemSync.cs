using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSync : MonoBehaviourPun
{
    [SerializeField] private List<ItemData> itemDataList;  // 아이템 데이터 리스트
    private string myId; // 임시 플레이어 식별 ID

    void Start()
    {
        // Todo: 플레이어 Id를 firebase 플레이어 UID로 변경
        myId = PhotonNetwork.LocalPlayer.NickName;

        // 아이템 리스트 전체 클라이언트에 동기화
        photonView.RPC("InitPlayerItems", RpcTarget.AllBuffered, myId);
    }

    public void UseItemRequest(string itemId)
    {
        photonView.RPC("UseItem", RpcTarget.AllViaServer, myId, itemId);
    }
    
    // 아이템 초기화
    [PunRPC]
    private void InitPlayerItems(string playerId)
    {
        Player player = FindLTHPlayer(playerId);
        var myItems = new List<ItemData>(itemDataList);
        ItemSyncManager.Instance.OnSyncReceived(player, myItems);
        Debug.Log($"[{playerId}] 아이템 생성 및 동기화 완료");
    }

    // 아이템 사용 처리
    [PunRPC]
    private void UseItem(string playerId, string itemId)
    {
        Player player = FindLTHPlayer(playerId);
        var items = ItemSyncManager.Instance.GetSyncedItems(player);
        var targetItem = items.Find(i => i.itemId == itemId);

        if (targetItem == null)
        {
            Debug.Log($"{playerId}에게 {itemId} 아이템 없음");
            return;
        }
    }
    
    // 아이템 상자 열기
    public void BoxOpen()
    {
        photonView.RPC("OpenBox", RpcTarget.AllBuffered);
    }

    // 아이템 상자 동기화
    [PunRPC]
    private void OpenBox()
    {
        var box = ItemBoxManager.Instance;
        box.OnBoxClicked();
    }
    private Player FindLTHPlayer(string playerId)
    {
        return FindObjectsOfType<Player>().FirstOrDefault(p => p.FirebaseUID == playerId);
    }
}
