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
            if (!itemDict.ContainsKey(item.itemId))
                itemDict.Add(item.itemId, item);
        }
    }

    public ItemData GetItemById(string id)
    {
        itemDict.TryGetValue(id, out var data);
        return data;
    }
}