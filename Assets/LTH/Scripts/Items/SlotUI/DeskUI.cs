using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스

/// <summary>
/// 특정 플레이어의 아이템 슬롯(책상)을 UI로 표현하는 클래스
/// </summary>

public class DeskUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> itemSlots; // 아이템 슬롯 리스트

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
}