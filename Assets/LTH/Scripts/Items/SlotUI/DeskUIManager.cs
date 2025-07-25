using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using Photon.Pun;

/// <summary>
/// 각 플레이어의 DeskUI를 관리하는 매니저 클래스
/// playerId(Firebase UID)를 기준으로 등록 및 조회 가능
/// </summary>

public class DeskUIManager : Singleton<DeskUIManager>
{
    [Header("UI 프리팹 및 배치 위치")]
    [SerializeField] private GameObject deskUIPrefab;
    [SerializeField] private Transform[] spawnPoints; // UI 배치 위치들

    private Dictionary<string, DeskUI> deskUIDict = new(); // playerId -> DeskUI 매핑

    private void Awake() => SingletonInit();

    /// <summary>
    /// 플레이어 수 만큼 DeskUI 생성 및 할당
    /// </summary>
    public void CreateDeskUIs(List<GamePlayer> players)
    {
        for (int i = 0; i < players.Count; i++) // 각 플레이어마다 반복 처리
        {
            var player = players[i];
            string playerId = player.PlayerId; // 플레이어 객체와 Firebase UID 획득하여 UID 기준으로 UI를 매핑

            Transform spawnPoint = spawnPoints.Length > i ? spawnPoints[i] : null; // UI가 생성될 위치 확보
            
            if (spawnPoint == null) // 스폰 위치가 없으면 에러 출력 후 해당 플레이어는 스킵
            {
                Debug.LogError($"[DeskUIManager] SpawnPoint 부족: Index {i} 없음");
                continue;
            }

            var deskObj = Instantiate(deskUIPrefab, spawnPoint.position, spawnPoint.rotation);
            var deskUI = deskObj.GetComponent<DeskUI>();

            if (deskUI == null)
            {
                Debug.LogError("[DeskUIManager] DeskUI 컴포넌트 없음"); // 생성한 오브젝트에 DeskUI 컴포넌트가 없으면 에러 출력
                continue;
            }

            deskUI.SetOwner(playerId); // 이 UI가 어떤 플레이어의 것인지 설정

            // 내 UI만 상호작용 허용
            bool isMine = player.GetComponent<PhotonView>().IsMine;
            deskUI.SetInteractable(isMine); // 내 책상 UI만 상호작용 가능하게 설정

            deskUIDict[playerId] = deskUI; // UID를 키로 하여 Dictionary에 DeskUI 등록
        }
    }

    /// <summary>
    /// 특정 플레이어의 DeskUI 가져오기
    /// </summary>
    public DeskUI GetDeskUI(string playerId)
    {
        if (deskUIDict.TryGetValue(playerId, out var deskUI))
            return deskUI;

        Debug.LogWarning($"[DeskUIManager] DeskUI 찾을 수 없음: {playerId}");
        return null;
    }

    /// <summary>
    /// 모든 DeskUI 제거 (씬 전환 등에서 호출)
    /// </summary>
    public void ClearAll()
    {
        foreach (var kv in deskUIDict)
        {
            Destroy(kv.Value.gameObject);
        }
        deskUIDict.Clear();
    }
}