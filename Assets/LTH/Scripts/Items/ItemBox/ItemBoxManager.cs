using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH;
using Photon.Pun;
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

    private string ownerNickname;
    public string OwnerNickname => ownerNickname;

    public bool IsMine => ownerNickname == PhotonNetwork.NickName;
    public int NewItemCount => newItemCount;

    private bool isOpened = false;
    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (photonView != null && photonView.Owner != null)
        {
            ownerNickname = photonView.Owner.NickName;
            Debug.Log($"[ItemBoxManager] 자동 OwnerNickname 설정됨: {ownerNickname}");
        }
    }

    /// <summary>
    /// 각 플레이어에 대한 초기화
    /// </summary>
    public void Init(DeskUI deskUI)
    {
        this.deskUI = deskUI;
        photonView.RPC(nameof(RPC_Init), RpcTarget.AllBuffered);
    }

    [PunRPC]
    private void RPC_Init()
    {
        Debug.Log($"[ItemBoxManager] Init 완료 → Owner: {ownerNickname}, Local: {PhotonNetwork.NickName}");
        CloseBoxImmediately();
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
        Debug.Log($"[BoxClicked] 호출됨 → {PhotonNetwork.NickName}, isOpened: {isOpened}, IsMine: {IsMine}");

        if (isOpened)
        {
            Debug.LogWarning("[BoxClicked] 이미 열린 상자입니다.");
            return;
        }

        if (!IsMine)
        {
            Debug.LogWarning("[BoxClicked] 상자 주인이 아닙니다.");
            return;
        }

        isOpened = true;;

        var rewards = ItemDatabaseManager.Instance.GetRandomItems(newItemCount);
        var rewardIds = new List<string>();

        foreach (var item in rewards)
        {
            if (item == null || string.IsNullOrEmpty(item.itemId))
            {
                Debug.LogError("[ItemBoxManager] 잘못된 아이템 감지됨");
                continue;
            }

            rewardIds.Add(item.itemId);
        }

        photonView.RPC(nameof(RPC_DistributeRewards), RpcTarget.All, string.Join(",", rewardIds));
    }

    [PunRPC]
    private void RPC_DistributeRewards(string joinedIds)
    {
        string[] itemIds = joinedIds.Split(',');
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

        photonView.RPC(nameof(RPC_DistributeRewards), RpcTarget.All, string.Join(",", rewardIds));
    }
}