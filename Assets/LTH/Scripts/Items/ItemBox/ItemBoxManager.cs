using DG.Tweening;
using LTH;
using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

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
    public string OwnerNickname
    {
        get => ownerNickname;
        set => ownerNickname = value;
    }

    public bool IsMine => OwnerNickname == PhotonNetwork.NickName;
    public int NewItemCount => newItemCount;

    private bool isOpened = false;
    public bool IsOpened => isOpened;

    private void Awake()
    {
        if (photonView != null && photonView.Owner != null)
        {
            ownerNickname = photonView.Owner.NickName;
            ItemBoxSpawnerManager.Instance.RegisterItemBox(ownerNickname, this);
        }
    }

    [PunRPC]
    public void SetOwnerNickname(string nickname)
    {
        OwnerNickname = nickname;
    }
    
    
    [PunRPC]
    private void RegisterItemBox()
    {
        Debug.Log($"[ItemBoxManager] {PhotonNetwork.LocalPlayer.NickName} 의 ItemBoxSpawnerManager에 {ownerNickname}의 ItemBoxManager를 등록합니다.");
        ItemBoxSpawnerManager.Instance.RegisterItemBox(ownerNickname, this);
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
        CloseBoxImmediately();
    }


    /// <summary>
    /// 상자 등장 (게임 매니저에서 호출)
    /// </summary>
    [PunRPC]
    public void RPC_ShowBox()
    {
        isOpened = false;
        gameObject.SetActive(true);
        itemBoxPrefabs.SetActive(true);

        CloseBoxImmediately(); // Dotween으로 아이템 상자 닫힌 상태 유지

        itemBoxPrefabs.transform.localScale = Vector3.zero; // 다음 등장 대비

        // 연출: 크기 0에서 등장
        itemBoxPrefabs.transform
       .DOScale(0.3f, 0.5f)
       .SetEase(Ease.OutBack);

        // 사운드: 상자 등장 효과음 재생
        // Manager.Sound.Play(" ", Define_LDH.Sound.Sfx);
    }

    public void ShowBox()
{
    photonView.RPC(nameof(RPC_ShowBox), RpcTarget.All);
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