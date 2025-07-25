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


    private void Awake()
    {
        if (uiEventHandler != null)
        {
            uiEventHandler.OnClickHandler += OnClick;
            uiEventHandler.OnEnterHandler += OnHoverEnter;
            uiEventHandler.OnExitHandler += OnHoverExit;
        }
    }

    #region 네트워크 아이템 배치
    /// <summary>
    /// 외부에서 호출: 아이템 ID를 통해 네트워크 동기화 요청
    /// </summary>
    public void PlaceItemById(string itemId)
    {
        photonView.RPC(nameof(RPC_PlaceItem), RpcTarget.All, itemId);
    }

    [PunRPC]
    private void RPC_PlaceItem(string itemId)
    {
        Clear();

        itemData = ItemDatabaseManager.Instance.GetItemById(itemId);
        if (itemData != null && itemData.itemPrefab != null)
        {
            currentItem = Instantiate(itemData.itemPrefab, anchorPoint.position, anchorPoint.rotation, anchorPoint);
            currentItem.transform.SetParent(anchorPoint, worldPositionStays: true);
        }
        else
        {
            Debug.LogWarning($"[ItemSlot] ItemData 또는 프리팹 없음: {itemId}");
        }
    }

    #endregion

    // TODO: 마우스를 갖다 대면 아이템 정보 표시 UI 추가 필요 (연결)
    #region 상호작용
    private void OnClick(PointerEventData eventData)
    {
        if (!IsEmpty())
        {
            UseItem();
        }
    }

    private void OnHoverEnter(PointerEventData eventData)
    {
        if (!IsEmpty() && itemData != null)
        {

        }
    }

    private void OnHoverExit(PointerEventData eventData)
    {
        Debug.Log("슬롯에서 마우스 이탈");
    }

    private void UseItem()
    {
        Debug.Log("아이템 사용!");
        Clear();
    }
    #endregion


    #region 유틸
    public void Clear()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }

    public bool IsEmpty() => currentItem == null;

    public void SetInteractable(bool interactable)
    {
        if (itemCollider != null)
        {
            itemCollider.enabled = interactable; // 특정 상황(예: 상대방 턴, 비활성화 슬롯 등)에서 상호작용 차단 가능
        }
    }
    #endregion
}