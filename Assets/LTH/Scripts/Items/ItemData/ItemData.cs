using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTH
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "Item/ItemData")]
    public class ItemData : ScriptableObject
    {
        public string itemId;
        public ItemType itemType;   // 아이템 유형
        public string displayName;  // UI 등에 표시할 이름
        public string description;  // 아이템 설명

        [NonSerialized] public string uniqueInstanceId; // 고유 ID
        [NonSerialized] public bool isUsed;             // 아이템 사용 여부

        [Header("Prefab")]
        public GameObject itemPrefab; // 아이템 프리팹

        public ItemData(string itemId, ItemType type, string name, string desc, GameObject prefab, bool used = false)
        {
            this.itemId = itemId;
            this.itemType = type;
            this.displayName = name;
            this.description = desc;
            this.itemPrefab = prefab;
            this.isUsed = used;
        }

        public ItemData Clone()
        {
            var clone = Instantiate(this);
            clone.uniqueInstanceId = Guid.NewGuid().ToString();
            return clone;
        }
    }
}