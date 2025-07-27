using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using DesignPattern;

public class ItemManager : Singleton<ItemManager>
{
    public void UseItem(ItemData item, GamePlayer user, GamePlayer target)
    {
        if (item == null || item.isUsed)
        {
            Debug.LogWarning("아이템이 null이거나 이미 사용됨");
            return;
        }

        ApplyEffect(item.itemType, user, target);
        SetItemUsed(item);
    }

    public void ApplyEffect(ItemType itemType, GamePlayer user, GamePlayer target)
    {
        switch (itemType)
        {
            case ItemType.Cigarette:
                user.IncreaseHp(1);
                Debug.Log("담배 사용 → 피1 회복");
                break;

            case ItemType.Cuffs:
                target.IsCuffedThisTurn = true;
                Debug.Log("수갑 사용 → 대상의 다음 턴 스킵 예약됨");
                break;

            case ItemType.Dial:
                //user.useDial();
                Debug.Log("다이얼 사용 → 현재 탄 타입 변환");
                break;

            case ItemType.Cellphone:
                //user.useCellphone();
                Debug.Log("휴대폰 사용 → 랜덤 탄 종류 알려줌");
                break;

            default:
                Debug.LogWarning("정의되지 않은 아이템 타입");
                break;
        }
    }

    public void SetItemUsed(ItemData item)
    {
        item.isUsed = true;
    }
}
