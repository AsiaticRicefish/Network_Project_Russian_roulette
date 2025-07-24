using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH; // �ӽ� Playerm ItemData ���ǿ� ���ӽ����̽�

/// <summary>
/// Ư�� �÷��̾��� ������ ����(å��)�� UI�� ǥ���ϴ� Ŭ����
/// </summary>

public class DeskUI : MonoBehaviour
{
    [SerializeField] private List<ItemSlot> itemSlots; // ������ ���� ����Ʈ

    public void Setup(List<ItemData> dataList) // ������ UI �ʱ� ����
    {
        for (int i = 0; i < itemSlots.Count; i++) // ���� ������ŭ �ݺ�
        {
            if (i < dataList.Count) // dataList�� �ش��ϴ� �������� ������ �� Init() ȣ��
            {
                itemSlots[i].gameObject.SetActive(true);
                itemSlots[i].Init(dataList[i]);
            }
            else
            {
                itemSlots[i].Clear(); // dataList�� �������� ������ �� Clear() ȣ��
            }
        }
    }

    // ToDo: Dotween �ִϸ��̼�, ���� ȿ�� �߰� ����
    public void Show() => gameObject.SetActive(true);
    public void Hide() => gameObject.SetActive(false);
}