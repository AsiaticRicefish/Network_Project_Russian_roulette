using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTH
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "LTH/ItemData")]
    public class ItemData : ScriptableObject
    {
        public ItemType itemType;   // 아이템 유형
        public string displayName;  // UI 등에 표시할 이름
        public string description;  // 아이템 설명
        public Sprite icon;         // 아이콘 이미지
        public bool isUsed;         // 아이템 사용 여부

        [Header("Prefab")]
        public GameObject itemPrefab; // 아이템 프리팹

        public ItemData(ItemType type, string name, string desc, bool used = false)
        {
            itemType = type;
            displayName = name;
            description = desc;
            isUsed = used;
        }
    }
}