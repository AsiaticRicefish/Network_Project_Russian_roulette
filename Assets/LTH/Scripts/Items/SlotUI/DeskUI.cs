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

    public void Setup(List<ItemData> dataList) // 아이템 UI 초기 세팅
    {
        for (int i = 0; i < itemSlots.Count; i++) // 슬롯 개수만큼 반복
        {
            if (i < dataList.Count) // dataList에 해당하는 아이템이 있으면 → Init() 호출
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].Init(dataList[i]);
            }
            else
            {
                itemSlots[i].Clear(); // dataList에 아이템이 없으면 → Clear() 호출
            }
        }
    }

    // ToDo: Dotween 애니메이션, 연출 효과 추가 예정
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}