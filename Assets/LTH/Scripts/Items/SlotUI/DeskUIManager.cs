using DesignPattern;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// 각 플레이어의 DeskUI를 관리하는 매니저 클래스
/// 기준 식별자는 Photon Nickname
/// </summary>

public class DeskUIManager : Singleton<DeskUIManager>
{
    [Header("UI 프리팹 및 배치 위치")]
    [SerializeField] private string deskUIPrefabPath = "ItemSlot/DeskCanvas";
    [SerializeField] private Transform[] spawnPoints; // UI 배치 위치들

    private Dictionary<string, DeskUI> deskUIDict = new(); // nickname → DeskUI

    private void Awake() => SingletonInit();

    /// <summary>
    /// 외부에서 생성된 DeskUI를 등록
    /// </summary>
    public void RegisterDeskUI(string nickname, DeskUI deskUI)
    {
        if (!deskUIDict.ContainsKey(nickname))
        {
            deskUIDict[nickname] = deskUI;
        }
    }

    /// <summary>
    /// 현재 클라이언트의 DeskUI를 직접 생성하고 등록
    /// </summary>
    public void CreateMyDeskUI()
    {
        int index = GetMyActorNumberIndex();
        if (index < 0 || index >= spawnPoints.Length)
        {
            return;
        }

        Transform spawnPoint = spawnPoints[index];
        GameObject deskObj = PhotonNetwork.Instantiate(deskUIPrefabPath, spawnPoint.position, spawnPoint.rotation);

        if (!deskObj.TryGetComponent(out DeskUI deskUI))
        {
            return;
        }

        string myNickname = PhotonNetwork.NickName;
        //deskUI.SetOwner(myNickname);
        deskUI.SetInteractable(true);
        //RegisterDeskUI(myNickname, deskUI);
    }

    /// <summary>
    /// Photon.ActorNumber를 기준으로 안정적인 인덱스 반환
    /// </summary>
    private int GetMyActorNumberIndex()
    {
        var sorted = PhotonNetwork.PlayerList.OrderBy(p => p.ActorNumber).ToList();
        return sorted.IndexOf(PhotonNetwork.LocalPlayer);
    }

    /// <summary>
    /// 닉네임으로 DeskUI 조회
    /// </summary>
    public DeskUI GetDeskUI(string nickname)
    {
        if (deskUIDict.TryGetValue(nickname, out var deskUI))
            return deskUI;

        return null;
    }

    /// <summary>
    /// 전체 DeskUI 제거
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in deskUIDict)
        {
            if (kv.Value != null)
                Destroy(kv.Value.gameObject);
        }
        deskUIDict.Clear();
    }
}