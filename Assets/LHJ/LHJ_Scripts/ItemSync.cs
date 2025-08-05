using LTH;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameUI;
using UnityEngine;


/// <summary>
/// 각 플레이어의 아이템 상태를 동기화하고, 상자와 연결하는 컴포넌트
/// 기준 식별자는 Photon Nickname
/// </summary>
public class ItemSync : MonoBehaviourPun
{
    [SerializeField] private List<ItemData> itemDataList;

    [HideInInspector] public ItemBoxManager myItemBox;

    
    public string MyNickname => photonView.Owner.NickName;
    public bool IsMine() => photonView.IsMine;


    private DeskUI myDeskUI;
    
    private void Start()
    {
        StartCoroutine(OnNetworkReady());
    }

    /// <summary>
    /// PhotonNetwork, SpawnerManager 초기화 기다린 뒤 InitPlayerItems 실행
    /// </summary>
    private IEnumerator OnNetworkReady()
    {
        
        yield return new WaitUntil(() =>
            PhotonNetwork.IsConnected &&
            DeskUIManager.Instance != null &&
            ItemBoxSpawnerManager.Instance != null);
        
        // 로컬에서만 데스크 Ui 생성 & deskuimanager에 등록
        if (IsMine())
        {
            DeskUIManager.Instance.CreateMyDeskUI();
            myDeskUI = DeskUIManager.Instance.GetDeskUI(MyNickname);

            #region Legacy

            //}
            // var myDeskUI = DeskUIManager.Instance.GetDeskUI(MyNickname);
            //
            // if (myDeskUI == null)
            // {
            //     Debug.LogWarningFormat($"[ItemSync] 내 DeskUI를 찾을 수 없음 → {MyNickname}");
            //     yield break;
            // }
            // if (IsMine())
            // {

            #endregion
            
            int spawnIndex = GetMySpawnIndex();
            var spawnPoints = ItemBoxSpawnerManager.Instance.GetComponentsInChildren<Transform>()
                .Where(t => t.CompareTag("ItemBoxSpawnPoint")).ToArray();

            if (spawnIndex >= spawnPoints.Length)
            {
                Debug.LogError($"[ItemSync] ItemBox 생성 위치 부족 → index: {spawnIndex}");
                yield break;
            }

            Transform spawnPos = spawnPoints[spawnIndex];
            GameObject obj = PhotonNetwork.Instantiate("ItemBox", spawnPos.position, spawnPos.rotation);

            var box = obj.GetComponent<ItemBoxManager>();
            if (box == null)
            {
                Debug.LogError("[ItemSync] ItemBoxManager 컴포넌트 없음!");
                yield break;
            }

            //box.Init(myDeskUI);  //rpc에서 호출하므로 주석처리
            box.photonView.RPC("SetOwnerNickname", RpcTarget.AllBuffered, MyNickname);
            
            //박스가 모든 user한테 등록되게 처리
            box.photonView.RPC("RegisterItemBox", RpcTarget.AllBuffered, PhotonNetwork.LocalPlayer.UserId);
            // myItemBox = box;

            Debug.Log($"[ItemSync] 내 ItemBox 생성 완료 → {MyNickname}");
            
        }
        // else
        // {
            // // 내 박스를 이미 생성한 마스터의 것을 가져와 Init만 (x) 각자 자기 박스 생성(마스터에서 생성하는게 아니라)
            // yield return new WaitUntil(() => ItemBoxSpawnerManager.Instance.TryGetItemBox(MyNickname, out _));
            //
            // if (ItemBoxSpawnerManager.Instance.TryGetItemBox(MyNickname, out var foundBox))
            // {
            //     foundBox.Init(myDeskUI);
            //     myItemBox = foundBox;
            //     Debug.Log($"[ItemSync] 박스 연결 완료 → {MyNickname}");
            // }
            // else
            // {
            //     Debug.LogError($"[ItemSync] 박스 연결 실패 → {MyNickname}");
            // }
        // }

        // 아이템 초기화 동기화
        photonView.RPC(nameof(InitPlayerItems), RpcTarget.AllBuffered, MyNickname);

        // 상호작용 설정은 본인만
        if (IsMine())
        {
            myDeskUI.SetInteractable(true);
            Debug.Log($"[ItemSync] 내 DeskUI 클릭 활성화 → {MyNickname}");

            foreach (var player in PhotonNetwork.PlayerList)
            {
                if (player.NickName == MyNickname) continue;

                var otherDeskUI = DeskUIManager.Instance.GetDeskUI(player.NickName);
                if (otherDeskUI != null)
                {
                    otherDeskUI.SetInteractable(false);
                    Debug.Log($"[ItemSync] 상대 DeskUI 클릭 비활성화 → {player.NickName}");
                }
            }
        }
        //yield return new WaitUntil(() =>
        //   PhotonNetwork.IsConnected &&
        //   DeskUIManager.Instance != null &&
        //   ItemBoxSpawnerManager.Instance != null);

        //if (IsMine())
        //{
        //    Debug.Log($"[ItemSync] 내 박스 생성 시도 시작 → {MyNickname}");
        //    // 내 DeskUI 생성
        //    DeskUIManager.Instance.CreateMyDeskUI();

        //    // DeskUI 객체 가져옴
        //    var myDeskUI = DeskUIManager.Instance.GetDeskUI(MyNickname);
        //    if (myDeskUI == null)
        //    {
        //        Debug.LogError($"[ItemSync] 내 DeskUI를 찾을 수 없음 → {MyNickname}");
        //        yield break;
        //    }

        //    // 내 ItemBox 생성 및 등록
        //    int spawnIndex = GetMySpawnIndex();
        //    var spawnPoints = ItemBoxSpawnerManager.Instance.GetComponentsInChildren<Transform>()
        //        .Where(t => t.CompareTag("ItemBoxSpawnPoint")).ToArray();

        //    if (spawnIndex >= spawnPoints.Length)
        //    {
        //        Debug.LogError($"[ItemSync] ItemBox 생성 위치 부족 → index: {spawnIndex}");
        //        yield break;
        //    }

        //    Transform spawnPos = spawnPoints[spawnIndex];
        //    Debug.Log($"[ItemSync] 박스 생성 위치: {spawnPos.position}");
        //    GameObject obj = PhotonNetwork.Instantiate("ItemBox", spawnPos.position, spawnPos.rotation);
        //    Debug.Log($"[ItemSync] PhotonNetwork.Instantiate 성공 → {obj.name}");
        //    var box = obj.GetComponent<ItemBoxManager>();
        //    if (box == null)
        //    {
        //        Debug.LogError("[ItemSync] ItemBoxManager 컴포넌트 없음!");
        //        yield break;
        //    }
        //    box.Init(myDeskUI);

        //    myItemBox = box;

        //    ItemBoxSpawnerManager.Instance.RegisterItemBox(MyNickname, box);
        //    Debug.Log($"[ItemSync] 내 박스 생성 및 등록 완료 → {MyNickname}");
        //}

        //// 아이템 초기화 RPC
        //photonView.RPC(nameof(InitPlayerItems), RpcTarget.AllBuffered, MyNickname);

        //// 마지막에 인터랙션 설정
        //if (IsMine())
        //{
        //    var myDeskUI = DeskUIManager.Instance.GetDeskUI(MyNickname);
        //    if (myDeskUI != null)
        //    {
        //        myDeskUI.SetInteractable(true);
        //        Debug.Log($"[ItemSync] 내 DeskUI 클릭 활성화 → {MyNickname}");
        //    }

        //    foreach (var player in PhotonNetwork.PlayerList)
        //    {
        //        if (player.NickName == MyNickname) continue;

        //        var otherDeskUI = DeskUIManager.Instance.GetDeskUI(player.NickName);
        //        if (otherDeskUI != null)
        //        {
        //            otherDeskUI.SetInteractable(false);
        //            Debug.Log($"[ItemSync] 상대 DeskUI 클릭 비활성화 → {player.NickName}");
        //        }
        //    }
        //}
    }

    

    [PunRPC]
    private void InitPlayerItems(string nickname)
    {
        var myItems = new List<ItemData>();

        foreach (var item in itemDataList)
        {
            var copy = Instantiate(item);     // 복제
            copy.isUsed = false;              // 상태 초기화
            myItems.Add(copy);
        }

        ItemSyncManager.Instance.OnSyncReceived(nickname, myItems);
        Debug.Log($"[ItemSync] 아이템 동기화 완료 → {nickname}");
    }


    public void UseItemRequest(string itemId)
    {
        photonView.RPC(nameof(UseItem), RpcTarget.AllViaServer, MyNickname, itemId);
    }

    [PunRPC]
    private void UseItem(string nickname, string itemId)
    {
        var items = ItemSyncManager.Instance.GetSyncedItems(nickname);
        var targetItem = items.FirstOrDefault(i => i.itemId == itemId);

        if (targetItem == null)
        {
            Debug.LogWarning($"[UseItem] {nickname}에게 {itemId} 아이템 없음");
            return;
        }

        Debug.Log($"[UseItem] {nickname} → {itemId} 사용");

        var user = FindPlayerByNickname(nickname);
        var target = FindTargetPlayer(user);

        ItemManager.Instance.UseItem(targetItem, user, target);

        photonView.RPC(nameof(ClearSlot), RpcTarget.All, nickname, itemId);
    }

    [PunRPC]
    private void ClearSlot(string nickname, string itemId)
    {
        var deskUI = DeskUIManager.Instance.GetDeskUI(nickname);
        if (deskUI == null)
        {
            Debug.LogWarning($"[ClearSlot] {nickname}의 DeskUI 없음");
            return;
        }

        deskUI.ClearItemSlotById(itemId);
    }

    public void BoxOpen()
    {
        if (myItemBox == null)
        {
            Debug.Log($"[ItemSync] 내 상자 찾기 실패: {MyNickname}");
            return;
        }

        if (myItemBox.IsOpened)
        {
            Debug.Log($"[ItemSync] 이미 열린 상자 → RPC 생략: {MyNickname}");
            return;
        }

        photonView.RPC(nameof(OpenBox), RpcTarget.AllBuffered, MyNickname);
    }

    [PunRPC]
    private void OpenBox(string nickname)
    {
        if (myItemBox == null)
        {
            Debug.Log($"[ItemSync] ItemBox 여전히 null → nickname: {nickname}");
            return;
        }

        if (myItemBox.OwnerNickname == nickname)
        {
            myItemBox.OnBoxClicked();
        }
        else
        {
            Debug.LogWarning($"[ItemSync] {nickname}는 이 상자의 주인이 아닙니다");
        }
    }

    private GamePlayer FindPlayerByNickname(string nickname)
    {
        return PlayerManager.Instance.FindPlayerByNickname(nickname);
    }

    private GamePlayer FindTargetPlayer(GamePlayer user)
    {
        return PlayerManager.Instance.GetAllPlayers()
            .Where(kvp => kvp.Key != user.PlayerId && kvp.Value.IsAlive)
            .Select(kvp => kvp.Value)
            .FirstOrDefault();
    }

    /// <summary>
    /// 현재 로컬 플레이어의 Photon Index 반환
    /// </summary>
    private int GetMySpawnIndex()
    {
        var list = PhotonNetwork.PlayerList;
        for (int i = 0; i < list.Length; i++)
        {
            if (list[i].NickName == PhotonNetwork.NickName)
                return i;
        }

        return -1;
    }
}