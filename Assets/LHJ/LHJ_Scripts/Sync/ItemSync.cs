using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSync : MonoBehaviourPun
{
    [SerializeField] private List<ItemData> itemDataList;  // 아이템 데이터 리스트
    [SerializeField] private ItemBoxManager myItemBox;

    private string myId; // 임시 플레이어 식별 ID

    public void Init(string playerId, ItemBoxManager box)
    {
        myId = playerId;
        myItemBox = box;
    }

    private void Start()
    {
        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        yield return new WaitForSeconds(0.5f);
        myId = PhotonNetwork.LocalPlayer.NickName;
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
        ItemSyncManager.Instance.OnSyncReceived(playerId, myItems);
        Debug.Log($"[{playerId}] 아이템 생성 및 동기화 완료");
    }

    // 아이템 사용 처리
    [PunRPC]
    private void UseItem(string playerId, string itemId)
    {
       // Player player = FindLTHPlayer(playerId);
        var items = ItemSyncManager.Instance.GetSyncedItems(playerId);
        var targetItem = items.Find(i => i.itemId == itemId);

        if (targetItem == null)
        {
            Debug.Log($"{playerId}에게 {itemId} 아이템 없음");
            return;
        }

        Debug.Log($"[UseItem] {playerId} → {itemId} 사용됨");
    }
    
    // 아이템 상자 열기
    public void BoxOpen()
    {
        photonView.RPC("OpenBox", RpcTarget.AllBuffered);
    }

    // 아이템 상자 동기화
    [PunRPC]
    private void OpenBox(string playerId)
    {
        if (myItemBox.OwnerId == playerId)
        {
            myItemBox.OnBoxClicked();
        }
    }
    private Player FindLTHPlayer(string playerId)
    {
        return FindObjectsOfType<Player>().FirstOrDefault(p => p.FirebaseUID == playerId);
    }
}