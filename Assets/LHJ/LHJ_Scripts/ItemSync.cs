using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemSync : MonoBehaviourPun
{
    [SerializeField] private List<ItemData> itemDataList;  // 아이템 데이터 리스트
    [HideInInspector] public ItemBoxManager myItemBox;

    private string myId; // 임시 플레이어 식별 ID

    public void Init(string playerId, ItemBoxManager box)
    {
        myId = playerId;
        myItemBox = box;

        Debug.Log($"[ItemSync] Init 완료 → playerId: {playerId}, box: {box.name}");
    }

    private void Start()
    {
        if (myItemBox == null)
        {
            Debug.LogWarning($"[ItemSync] myItemBox 자동 연결 실패! Init 호출이 누락된 것 같습니다.");
        }

        StartCoroutine(DelayedInit());
    }

    private IEnumerator DelayedInit()
    {
        // 닉네임이 세팅될 때까지 대기
        while (string.IsNullOrEmpty(PhotonNetwork.LocalPlayer.NickName)) yield return null;
        myId = PhotonNetwork.LocalPlayer.NickName;

        // ItemBoxSpawnerManager가 생성되었는지 대기
        yield return new WaitUntil(() => ItemBoxSpawnerManager.Instance != null);

        // 자동 연결 시도 (반드시 필요)
        if (myItemBox == null)
        {
            var candidates = FindObjectsOfType<ItemBoxManager>();
            foreach (var box in candidates)
            {
                if (box.OwnerId == myId)
                {
                    myItemBox = box;
                    Debug.Log($"[ItemSync] 내 상자 연결 완료 → {myItemBox.name}");
                    break;
                }
            }

            // 실제 상자 존재하는지 체크 후 Init 시도
            if (ItemBoxSpawnerManager.Instance.TryGetItemBox(myId, out var myBox))
            {
                Init(myId, myBox);
                Debug.Log($"[ItemSync] {myId} 상자 연결 완료");
            }
            else
            {
                Debug.LogError($"[ItemSync] 내 상자를 찾을 수 없음! → myId: {myId}");
            }
        }

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
        var items = ItemSyncManager.Instance.GetSyncedItems(playerId);
        var targetItem = items.Find(i => i.itemId == itemId);

        if (targetItem == null)
        {
            Debug.Log($"{playerId}에게 {itemId} 아이템 없음");
            return;
        }

        Debug.Log($"[UseItem] {playerId} → {itemId} 사용됨");

        switch (targetItem.itemType)
        {
            case ItemType.Cigarette:
                Debug.Log("체력 +1");
                break;
            case ItemType.Saw:
                Debug.Log("총 데미지 2배!");
                break;
        }
    }
    
    // 아이템 상자 열기
    public void BoxOpen()
    {
        if (myItemBox == null)
        {
            var candidates = FindObjectsOfType<ItemBoxManager>();
            foreach (var box in candidates)
            {
                if (box.OwnerId == myId)
                {
                    myItemBox = box;
                    break;
                }
            }

            if (myItemBox == null)
            {
                Debug.LogError($"[ItemSync] myItemBox를 찾을 수 없습니다. → {myId}");
                return;
            }
        }

        photonView.RPC("OpenBox", RpcTarget.AllBuffered, myId);
    }

    // 아이템 상자 동기화
    [PunRPC]
    private void OpenBox(string playerId)
    {
        if (myItemBox == null)
        {
            Debug.LogError($"[ItemSync] myItemBox가 연결되지 않았습니다. → playerId: {playerId}");
            return;
        }

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