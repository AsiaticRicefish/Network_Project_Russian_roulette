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

    [SerializeField] private string ownerNickname;
    [field: SerializeField] public string OwnerNickname
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
    private void RegisterItemBox(string userID)
    {
        Debug.Log($"[ItemBoxManager] {PhotonNetwork.LocalPlayer.NickName} 의 ItemBoxSpawnerManager에 {ownerNickname}의 ItemBoxManager를 등록합니다.");
        
        //desk ui init
        var myDeskUI = DeskUIManager.Instance.GetDeskUI(ownerNickname);
        Init(myDeskUI);
        
        //spawn manager에 register 하기
        ItemBoxSpawnerManager.Instance.RegisterItemBox(ownerNickname, this);

        // 오직 자기 자신인 경우에만 연결
        if (PhotonNetwork.LocalPlayer.UserId != userID) return;
   
        //각 플레이어의 itemsync의 myItemBox에 현재 itemBox 할당하기
        if (Manager.PlayerManager.GetAllPlayers().TryGetValue(userID, out GamePlayer gamePlayer))
        {
            gamePlayer.GetComponent<ItemSync>().myItemBox = this;
            Debug.Log($"[ItemBoxManager] myItemBox 할당 완료");
        }
        else
        {
            Debug.Log($"[ItemBoxManager] PlayerManager의 _players에서 {userID}로 된 GamePlayer를 찾을 수 없습니다.");
            return;
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

        itemBoxPrefabs.transform.localScale = Vector3.zero;

        itemBoxPrefabs.transform
       .DOScale(0.3f, 0.5f)
       .SetEase(Ease.OutBack);
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
        var ids = new List<string>();
        var uniqueIds = new List<string>();

        foreach (var item in rewards)
        {
            var instance = item.Clone();
            ids.Add(instance.itemId);
            uniqueIds.Add(instance.uniqueInstanceId);
        }

        photonView.RPC(nameof(RPC_DistributeRewards), RpcTarget.All, string.Join(",", ids), string.Join(",", uniqueIds));
    }

    [PunRPC]
    private void RPC_DistributeRewards(string joinedIds, string joinedUniqueIds)
    {
        string[] itemIds = joinedIds.Split(',');
        string[] uniqueIds = joinedUniqueIds.Split(',');

        isOpened = true;

        lidTransform.DOLocalRotate(openRotation, 0.7f)
            .SetEase(Ease.OutBack)
            .OnComplete(() =>
            {
                var rewardItems = new List<ItemData>();
                for (int i = 0; i < itemIds.Length && i < uniqueIds.Length; i++)
                {
                    var template = ItemDatabaseManager.Instance.GetItemById(itemIds[i]);
                    var instance = template.Clone();
                    instance.uniqueInstanceId = uniqueIds[i]; // 동기화된 고유 ID로 맞춰줌
                    rewardItems.Add(instance);
                }

                AutoPlaceToSlots(rewardItems);
                StartCoroutine(HideBoxAfterDelay());
            });
    }

    /// <summary>
    /// 빈 슬롯에 아이템 자동 배치
    /// </summary>
    private void AutoPlaceToSlots(List<ItemData> items)
    {
        var emptySlots = deskUI.GetEmptySlots();
        for (int i = 0; i < items.Count && i < emptySlots.Count; i++)
        {
            emptySlots[i].PlaceItemByInstance(items[i]);
        }
        
        //소리 연출 추가
        if(IsMine)
            Manager.Sound.PlaySfxByKey("ItemSpawn");
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
        
        Debug.Log("hide box after delay 호출");
        yield return new WaitForSeconds(1.5f);
        Debug.Log("1.5초 대기 완료");
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