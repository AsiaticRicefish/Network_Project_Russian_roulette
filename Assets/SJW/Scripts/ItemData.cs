using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewItem", menuName = "Item/ItemData")]
public class ItemData : ScriptableObject
{
    public ItemType itemType;            // 효과 구분용 enum
    public string displayName;           // 아이템 이름
    [TextArea] public string description; // 설명
    public Sprite icon;                  // 아이템 이미지

    [HideInInspector] public bool isUsed; // 런타임 중 사용 여부 (프로토용)
}
