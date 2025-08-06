using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH;
using DesignPattern;

public class ItemDatabaseManager : Singleton<ItemDatabaseManager>
{
    [SerializeField] private List<ItemData> allItems;

    private Dictionary<string, ItemData> itemDict;

    private void Awake()
    {
        SingletonInit();
        itemDict = new();

        foreach (var item in allItems)
        {
            if (item != null && !string.IsNullOrEmpty(item.itemId) && item.itemPrefab != null)
            {
                if (!itemDict.ContainsKey(item.itemId))
                    itemDict.Add(item.itemId, item);
            }
            else
            {
                Debug.LogWarning("[ItemDatabaseManager] 유효하지 않은 아이템 발견 (null, itemId 없음 또는 프리팹 없음)");
            }
        }
    }

    public ItemData GetItemById(string id)
    {
        itemDict.TryGetValue(id, out var data);
        return data?.Clone();
    }

    public List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> validItems = allItems.FindAll(item =>
       item != null && !string.IsNullOrEmpty(item.itemId) && item.itemPrefab != null);

        List<ItemData> result = new();
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, validItems.Count);
            result.Add(validItems[rand].Clone());
        }
        return result;
    }
}