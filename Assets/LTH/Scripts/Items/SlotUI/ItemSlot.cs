using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using LTH; // �ӽ� Playerm ItemData ���ǿ� ���ӽ����̽�

/// <summary>
///  UI �󿡼� ������ 1���� ǥ���ϴ� ���� (�� ���� ���� �ϳ����� ��ġ�ǵ���)
/// </summary>

public class ItemSlot : MonoBehaviour
{
    [Header("UI")]
    [SerializeField] private Image itemIconImage; // ������ ������
    [SerializeField] private TMP_Text itemNameText; // ������ �̸�
    [SerializeField] private Button useButton; // ���� Ŭ�� ��ư (����)

    private ItemData itemData;

    public void Init(ItemData data) // ���� �ʱ�ȭ (������ ����)
    {
        itemData = data; // ScriptableObject ����� ItemData�� �޾� ������ �ʱ�ȭ

        if (itemData == null)  return;

        itemNameText.text = itemData.displayName;
        itemIconImage.sprite = itemData.icon;

        // ��ư Ŭ�� �̺�Ʈ ����
        useButton.onClick.RemoveAllListeners();
        useButton.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        // TdDo : ������ ��� ó�� (ItemManager)
        // PhotonView�� ������ �ٸ� Ŭ���̾�Ʈ�� ������ ��� ����ȭ ó�� ����
    }

    public void Clear()
    {
        itemData = null;
        itemNameText.text = "";
        itemIconImage.sprite = null;
        useButton.onClick.RemoveAllListeners();
    }
}