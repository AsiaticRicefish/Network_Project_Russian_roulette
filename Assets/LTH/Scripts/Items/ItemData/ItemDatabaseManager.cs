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
            if (item.itemId!=null && !itemDict.ContainsKey(item.itemId))
                itemDict.Add(item.itemId, item);
        }
    }

    public ItemData GetItemById(string id)
    {
        itemDict.TryGetValue(id, out var data);
        return data;
    }

    public List<ItemData> GetRandomItems(int count)
    {
        List<ItemData> result = new();
        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, allItems.Count);
            result.Add(allItems[rand]); // 중복 허용
        }
        return result;
    }
}