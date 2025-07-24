using LTH; // 임시 Playerm ItemData 정의용 네임스페이스
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


/// <summary>
///  UI 상에서 아이템 1개를 표현하는 슬롯 (한 슬롯 마다 하나씩만 배치되도록)
/// </summary>
public class ItemSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Transform anchorPoint; // 아이템이 놓일 위치

    private GameObject currentItem;

    /// <summary>
    /// 아이템 프리팹을 슬롯 위에 배치
    /// </summary>
    public void PlaceItem(GameObject itemPrefab)
    {
        Clear();

        if (itemPrefab != null)
        {
            currentItem = Instantiate(itemPrefab, anchorPoint.position, anchorPoint.rotation, anchorPoint);
        }
    }

    public void Clear()
    {
        if (currentItem != null)
        {
            Destroy(currentItem);
            currentItem = null;
        }
    }

    public bool IsEmpty() => currentItem == null;
}