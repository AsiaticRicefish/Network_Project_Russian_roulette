using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using LTH; // �ӽ� Playerm ItemData ���ǿ� ���ӽ����̽�
// using Photon.Pun;
// using Photon.Realtime;
// using ExitGames.Client.Photon;

/// <summary>
/// ������ ����ȭ ���� Ŭ����
/// </summary>

public class ItemSyncManager : Singleton<ItemSyncManager>
{
    // �� �÷��̾ ����ȭ�� ������ ��� ����
    private Dictionary<Player, List<ItemData>> syncedItems = new();

    private void Awake()
    {
        SingletonInit();
    }

    /// <summary>
    /// �������� �����ϰ� ��Ʈ��ũ�� �����ϴ� ����  (���ÿ��� �����ϰ� ��Ʈ��ũ ���� ����)
    /// </summary>
    public void GenerateAndSync(Player player)
    {
        // ToDo: ���� ���� ������ �´� ������ ���� �������� ��ü �ʿ�
        var generatedItems = new List<ItemData>
        {
            new ItemData(ItemType.Cigarette, "���", "�÷��̾� ü�� 1 ȸ���մϴ�."),
            new ItemData(ItemType.Cellphone, "�޴���", "������ źȯ�� ���� �̷� ������ �����մϴ�.")
        };

        // ���ÿ� ������ �������� ����
        syncedItems[player] = generatedItems;

        // ToDo: �� ������ ����Ʈ�� ��Ʈ��ũ�� �ٸ� Ŭ���̾�Ʈ���� �����ؾ� ��
        // RPC�� ���� �� SerializeItemList() ���� ������� ����ȭ �ʿ�
        Debug.Log($"[ItemSyncManager] ������ ���� �� ����ȭ��: {player.FirebaseUID}"); ;
    }

    /// <summary>
    /// �ٸ� �÷��̾ ���� �������� �޾Ƽ� ����
    /// </summary>
    public void OnSyncReceived(Player player, List<ItemData> items)
    {
        // ��Ʈ��ũ�� ���� ���� ������ �����͸� �ش� �÷��̾ ����
        syncedItems[player] = items;
        // ToDo: �� �޼���� Photon�� OnEvent �Ǵ� RPC ���� �� ȣ��Ǿ�� ��
        Debug.Log($"[ItemSyncManager] ������ ����ȭ ���ŵ�: {player.FirebaseUID}");
    }

    /// <summary>
    /// Ư�� �÷��̾ ���� � �������� ����ȭ�޾Ҵ��� Ȯ��
    /// </summary>
    public List<ItemData> GetSyncedItems(Player player)
    {
        // Dictionary�� �����ϴ� ��� �ش� ������ ��� ��ȯ
        if (syncedItems.TryGetValue(player, out var items)) return items;

        // ������ �� ����Ʈ ��ȯ (null ����)
        return new List<ItemData>();
    }
}