using GameUI;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using Photon.Pun;


/// <summary>
///  UI 상에서 아이템 1개를 표현하는 슬롯 (네트워크 동기화 포함)
/// </summary>
public class ItemSlot : MonoBehaviourPun
{
    [Header("UI")]
    [SerializeField] private Transform anchorPoint; // 아이템이 놓일 위치
    [SerializeField] private Collider itemCollider; // 상호작용 차단용
    [SerializeField] private UI_EventHandler uiEventHandler;  // 클릭/호버 이벤트 처리용

    private GameObject currentItem;
    private ItemData itemData; // 호버 및 클릭 시 아이템 정보를 활용할 수 있도록

    private ItemSync itemSync; // 주입받는 객체
    public bool IsEmpty => currentItem == null;

    private string ownerNickname;

    public bool HasItemId(string id)
    {
        return itemData != null && itemData.itemId == id;
    }

    private void Awake()
    {
        // UI_EventHandler 자동 연결
        if (uiEventHandler == null)
        {
            uiEventHandler = GetComponent<UI_EventHandler>();
            if (uiEventHandler == null)
            {
                uiEventHandler = gameObject.AddComponent<UI_EventHandler>();
                Debug.LogWarning("[ItemSlot] UI_EventHandler가 없어서 자동 추가됨");
            }
        }

        // 클릭/호버 이벤트 등록
        uiEventHandler.OnClickHandler += OnClick;
        uiEventHandler.OnEnterHandler += OnHoverEnter;
        uiEventHandler.OnExitHandler += OnHoverExit;

        // Collider 자동 연결
        if (itemCollider == null)
        {
            itemCollider = GetComponent<Collider>();
            if (itemCollider == null)
            {
                itemCollider = gameObject.AddComponent<BoxCollider>();
                Debug.LogWarning("[ItemSlot] Collider가 없어서 BoxCollider 자동 추가됨");
            }
        }

        // Anchor 자동 연결
        if (anchorPoint == null)
        {
            var anchor = transform.Find("Anchor")?.GetComponent<RectTransform>();
            if (anchor != null)
            {
                anchorPoint = anchor;
                Debug.Log("[ItemSlot] AnchorPoint 자동 연결 완료");
            }
            else
            {
                Debug.LogWarning("[ItemSlot] AnchorPoint(자식에 'Anchor' 이름) 자동 연결 실패");
            }
        }
    }

    public void SetOwner(string nickname)
    {
        ownerNickname = nickname;
        TryBindItemSync(); // nickname 기준으로 다시 연결
    }

    private void TryBindItemSync()
    {
        if (itemSync != null || string.IsNullOrEmpty(ownerNickname)) return;

        var allSyncs = FindObjectsOfType<ItemSync>();
        foreach (var sync in allSyncs)
        {
            Debug.Log($"[ItemSlot] 검사 중: {sync.MyNickname} vs {ownerNickname}");
            if (sync.MyNickname == ownerNickname)
            {
                itemSync = sync;
                Debug.Log($"[ItemSlot] ItemSync 연결 완료 → {itemSync.MyNickname}");
                break;
            }
        }

        if (itemSync == null)
        {
            Debug.LogWarning($"[ItemSlot] ItemSync 연결 실패 → {ownerNickname}");
        }
    }

    /// <summary>
    /// 네트워크로 모든 클라이언트에게 아이템 배치 요청
    /// </summary>
    public void PlaceItemById(string itemId)
    {
        if (string.IsNullOrEmpty(itemId))
        {
            Debug.LogWarning("[ItemSlot] itemId가 유효하지 않음");
            return;
        }

        photonView.RPC(nameof(RPC_PlaceItem), RpcTarget.All, itemId);
    }

    [PunRPC]
    private void RPC_PlaceItem(string itemId)
    {
        ClearSlot();

        itemData = ItemDatabaseManager.Instance.GetItemById(itemId);
        if (itemData != null && itemData.itemPrefab != null)
        {
            Debug.Log($"[ItemSlot] 아이템 생성 시도: {itemData.itemPrefab.name}");
            string path = "Items/" + itemData.itemPrefab.name;
            currentItem = PhotonNetwork.Instantiate(path, anchorPoint.position, anchorPoint.rotation);
            currentItem.transform.SetParent(anchorPoint);
        }
        else
        {
            Debug.LogWarning($"[ItemSlot] ItemData 또는 프리팹 없음: {itemId}");
        }
    }

    /// <summary>
    /// 클릭 → 아이템 사용
    /// </summary>
    private void OnClick(PointerEventData eventData)
    {
        Debug.Log($"[ItemSlot] 클릭 시도됨 → {ownerNickname}");
        if (!IsEmpty && itemData != null)
        {
            if (itemSync == null)
            {
                Debug.LogWarning("[ItemSlot] itemSync 미연결 → 클릭 무시");
                return;
            }

            if (!itemSync.IsMine())
            {
                Debug.LogWarning("[ItemSlot] 내 슬롯이 아님 → 클릭 무시");
                return;
            }

            Debug.Log("[ItemSlot] 슬롯 클릭됨 → 아이템 사용");
            UseItem();
        }
    }

    private void UseItem()
    {
        if (itemSync == null)
        {
            Debug.LogWarning("[ItemSlot] ItemSync 연결 안됨");
            return;
        }

        Debug.Log($"[ItemSlot] 아이템 사용 요청: {itemData.itemId}");
        itemSync.UseItemRequest(itemData.itemId);

        // 슬롯 비우기 동기화
        photonView.RPC(nameof(RPC_Clear), RpcTarget.All);
    }

    [PunRPC]
    private void RPC_Clear()
    {
        ClearSlot();
    }

    public void ClearSlot()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }

        itemData = null;
    }

    /// <summary>
    /// 슬롯 상호작용 여부 설정
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        if (itemCollider != null)
        {
            itemCollider.enabled = interactable;
            Debug.Log($"[ItemSlot] Collider {(interactable ? "활성화" : "비활성화")}");
        }
    }

    private void OnHoverEnter(PointerEventData eventData)
    {
        if (!IsEmpty && itemData != null)
        {
            Debug.Log($"[ItemSlot] Hover → {itemData.displayName}");
        }
    }

    private void OnHoverExit(PointerEventData eventData)
    {
        Debug.Log($"[ItemSlot] Hover Exit");
    }
}