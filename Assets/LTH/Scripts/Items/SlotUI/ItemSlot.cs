using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스

/// <summary>
///  UI 상에서 아이템 1개를 표현하는 슬롯 (한 슬롯 마다 하나씩만 배치되도록)
/// </summary>

public class ItemSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIconImage; // 아이템 아이콘
    [SerializeField] private TMP_Text itemNameText; // 아이템 이름
    [SerializeField] private Button useButton; // 슬롯 클릭 버튼 (감지)

    private ItemData itemData;

    public void Init(ItemData data) // 슬롯 초기화 (데이터 세팅)
    {
        itemData = data; // ScriptableObject 기반의 ItemData를 받아 슬롯을 초기화

        if (itemData == null)  return;

        itemNameText.text = itemData.displayName;
        itemIconImage.sprite = itemData.icon;

        // 버튼 클릭 이벤트 설정
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        // TdDo : 아이템 사용 처리 (ItemManager)
        // PhotonView와 연동해 다른 클라이언트와 아이템 사용 동기화 처리 예정
    }

    public void Clear()
    {
        itemData = null;
        itemNameText.text = "";
        itemIconImage.sprite = null;
        useButton.onClick.RemoveAllListeners();
    }
}