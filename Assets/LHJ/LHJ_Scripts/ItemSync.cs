using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSync : MonoBehaviourPun
{
    [SerializeField] private List<ItemData> itemDataList;  // 아이템 데이터 리스트
    private string myId; // 임시 플레이어 식별 ID

    void Start()
    {
        // Todo: 플레이어 Id를 firebase 플레이어 UID로 변경
        // MasterClient면 플레이어1 아니면 player2(Test)
        myId = PhotonNetwork.IsMasterClient ? "player1" : "player2";

        // 아이템 리스트 전체 클라이언트에 동기화
        photonView.RPC("InitPlayerItems", RpcTarget.AllBuffered, myId);
    }

    void Update()
    { 
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            photonView.RPC("UseItem", RpcTarget.AllViaServer, myId, "담배");
        }

        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            photonView.RPC("UseItem", RpcTarget.AllViaServer, myId, "휴대폰");
        }
    }
    [PunRPC]
    private void InitPlayerItems(string playerId)
    {
        var myItems = new List<ItemData>(itemDataList);
        TestCopyItemSync.Instance.OnSyncReceived(playerId, myItems);
        Debug.Log($"[{playerId}] 아이템 생성 및 동기화 완료");
    }

    // 모든 클라이언트에서 호출
    [PunRPC]
    private void UseItem(string playerId, string itemName)
    {
        // 지정된 플레이어 ID에 해당하는 아이템 리스트 불러오기
        var items = TestCopyItemSync.Instance.GetSyncedItems(playerId);

        // 이름으로 아이템 찾기
        var targetItem = items.Find(i => i.displayName == itemName);

        // 해당 아이템이 없을때 
        if (targetItem == null)
        {
            Debug.Log($"{playerId}에게 {itemName} 아이템 없음");
            return;
        }

        // 아이템 제거 후 동기화
        items.Remove(targetItem);
        TestCopyItemSync.Instance.OnSyncReceived(playerId, items);
        Debug.Log($"{playerId}가 {itemName}을 사용함");
    }
}
