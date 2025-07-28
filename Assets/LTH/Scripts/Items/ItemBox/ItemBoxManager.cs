using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스
using Photon.Pun;
using DesignPattern; 
using DG.Tweening;

/// <summary>
/// 턴 종료 시 등장하는 보상 상자 매니저
/// 클릭 시 상자가 열리며, 아이템이 자동으로 빈 슬롯에 배치됨
/// </summary>

public class ItemBoxManager : MonoBehaviourPun
{
    [Header("TurnEnd NewItem")]
    [SerializeField] private int newItemCount;

    [Header("ItemBox Object")]
    [SerializeField] private GameObject itemBoxPrefabs;     // 상자 전체 오브젝트
    [SerializeField] private Transform lidTransform;        // 상자 뚜껑 (CommonMediumLid)
    [SerializeField] private Vector3 openRotation = new Vector3(-120f, 0f, 0f); // 뚜껑 열리는 각도

    [Header("Connect Slot")]
    [SerializeField] private DeskUI deskUI;
    
    private string ownerId; // 각 플레이어의 DeskUI와 연결하기 위한 소유자 ID
    public string OwnerId => ownerId;

    private bool isOpened = false;
    public bool IsMine => ownerId == PhotonNetwork.LocalPlayer.NickName;
    public int NewItemCount => newItemCount;

    /// <summary>
    /// 각 플레이어에 대한 초기화
    /// </summary>
    public void Init(string playerId, DeskUI deskUI)
    {
        ownerId = playerId;
        this.deskUI = deskUI;
        CloseBoxImmediately(); // 시작 시 닫힌 상태로 초기화
    }

    /// <summary>
    /// 상자 등장 (게임 매니저에서 호출)
    /// </summary>
    public void ShowBox()
    {
        isOpened = false;
        gameObject.SetActive(true);

        CloseBoxImmediately(); // Dotween으로 아이템 상자 닫힌 상태 유지
    }

    /// <summary>
    /// 상자를 닫힌 상태로 초기화
    /// </summary>
    private void CloseBoxImmediately()
    {
        lidTransform.localRotation = Quaternion.identity; // 뚜껑 닫기
        itemBoxPrefabs.SetActive(true);
    }


    /// <summary>
    /// 클릭 시 상자 열리고 보상 아이템 지급
    /// </summary>
    public void OnBoxClicked()
    {
        if (isOpened || !IsMine) return;

        isOpened = true;
        var rewards = ItemDatabaseManager.Instance.GetRandomItems(newItemCount);
        var rewardIds = new List<string>();
        foreach (var item in rewards)
            rewardIds.Add(item.itemId);

        photonView.RPC(nameof(RPC_DistributeRewards), RpcTarget.All, rewardIds.ToArray());
    }

    [PunRPC]
    private void RPC_DistributeRewards(string[] itemIds)
    {
        isOpened = true;

        lidTransform.DOLocalRotate(openRotation, 0.7f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                AutoPlaceToSlots(itemIds);
                StartCoroutine(HideBoxAfterDelay());
            });
    }

    /// <summary>
    /// 빈 슬롯에 아이템 자동 배치
    /// </summary>
    private void AutoPlaceToSlots(string[] itemIds)
    {
        var emptySlots = deskUI.GetEmptySlots();
        for (int i = 0; i < itemIds.Length && i < emptySlots.Count; i++)
        {
            emptySlots[i].PlaceItemById(itemIds[i]);
        }
    }

    public List<ItemData> PickRandomRewards()
    {
        return ItemDatabaseManager.Instance.GetRandomItems(newItemCount);
    }

    /// <summary>
    /// 연출 후 상자 숨김
    /// </summary>
    private IEnumerator HideBoxAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        itemBoxPrefabs.SetActive(false);
        gameObject.SetActive(false);
    }

    public void ShowBoxWithCustomItems(List<ItemData> customList)
    {
        isOpened = false;
        gameObject.SetActive(true);
        CloseBoxImmediately();

        var rewardIds = new List<string>();
        foreach (var item in customList)
            rewardIds.Add(item.itemId);

        photonView.RPC(nameof(RPC_DistributeRewards), RpcTarget.All, rewardIds.ToArray());
    }
}