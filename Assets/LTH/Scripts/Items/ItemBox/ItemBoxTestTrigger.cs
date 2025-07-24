using LTH;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;

public class ItemBoxTestTrigger : MonoBehaviour
{
    [SerializeField] private List<ItemData> testItems; // 테스트용 아이템 리스트
    [SerializeField] private ItemBoxManager itemBoxManager;

    private void Start()
    {
        // 테스트 아이템을 Manager에 등록
        itemBoxManager.ShowBoxWithCustomItems(testItems);
    }

    // 또는 키 입력으로 테스트 가능
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B)) // B 키를 누르면 상자 등장
        {
            itemBoxManager.ShowBoxWithCustomItems(testItems);
        }
    }
}