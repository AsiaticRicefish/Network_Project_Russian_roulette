using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LTH
{
    [CreateAssetMenu(fileName = "NewItemData", menuName = "LTH/ItemData")]
    public class ItemData : ScriptableObject
    {
        public ItemType itemType;   // ������ ����
        public string displayName;  // UI � ǥ���� �̸�
        public string description;  // ������ ����
        public Sprite icon;         // ������ �̹���
        public bool isUsed;         // ������ ��� ����

        public ItemData(ItemType type, string name, string desc, bool used = false)
        {
            itemType = type;
            displayName = name;
            description = desc;
            isUsed = used;
        }
    }
}