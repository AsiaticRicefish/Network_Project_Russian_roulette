using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스
using GameUI;

/// <summary>
/// 특정 플레이어의 아이템 슬롯(책상)을 UI로 표현하는 클래스
/// </summary>

public class DeskUI : UI_Base
{
    [SerializeField] private List<ItemSlot> itemSlots; // 아이템 슬롯 리스트

    private string ownerPlayerId;
    private bool isInteractable = true;

    public string OwnerPlayerId => ownerPlayerId;
    public bool IsInteractable => isInteractable;

    /// <summary>
    /// UI 초기화 시  null일 경우 슬롯 자동 등록
    /// </summary>
    protected override void Init()
    {
        if (itemSlots == null || itemSlots.Count == 0)
            itemSlots = new List<ItemSlot>(GetComponentsInChildren<ItemSlot>(includeInactive: true));
    }

    /// <summary>
    /// DeskUI가 어느 플레이어의 UI인지 식별
    /// 네트워크 기반 시스템에서 각 DeskUI를 playerId로 매핑 가능가능하도록 하기 위함
    /// </summary>
    public void SetOwner(string playerId)
    {
        ownerPlayerId = playerId;
    }

    /// <summary>
    /// 모든 슬롯 초기화 (아이템 제거)
    /// </summary>
    public void ClearAllSlots()
    {
        foreach (var slot in itemSlots)
        {
            if (slot != null) slot.Clear();
        }
    }

    /// <summary>
    /// 비어 있는 슬롯만 리스트로 반환
    /// </summary>
    public List<ItemSlot> GetEmptySlots()
    {
        return itemSlots.FindAll(slot => slot != null && slot.IsEmpty());
    }

    /// <summary>
    /// 턴제 구조 또는 특정 상황(예: 상대방 턴일 때)에서 아이템 슬롯 클릭/조작을 비활성화할 수 있도록 설계
    /// </summary>
    public void SetInteractable(bool interactable)
    {
        isInteractable = interactable;
        foreach (var slot in itemSlots)
        {
            if (slot != null) slot.SetInteractable(interactable);
        }
    }
}