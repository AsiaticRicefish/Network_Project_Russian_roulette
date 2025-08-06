using DG.Tweening;
using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class BulletDisplayer : MonoBehaviour
{
    [Header("배치 기준")]
    [SerializeField] private Transform anchorPoint;     // 테이블 중앙 기준
    [SerializeField] private float spacing = 0.3f;      // 총알 간 거리
    [SerializeField] private Vector3 offset = Vector3.up * 0.1f; // 약간 띄워주는 위치 조정

    [Header("총알 프리팹")]
    [SerializeField] private GameObject liveBulletPrefab;
    [SerializeField] private GameObject blankBulletPrefab;

    [Header("UI 연결")]
    [SerializeField] private GameObject bulletInfoPanel; // 전체 패널 (배경 포함)
    [SerializeField] private TextMeshProUGUI bulletCountText;

    [Header("UI 연출 설정")]
    [SerializeField] private float showDuration = 2.5f;   // 표시 시간
    [SerializeField] private float fadeDuration = 0.5f;   // 등장/퇴장 애니메이션 시간

    private List<GameObject> spawnedBullets = new();

    [SerializeField] private PhotonView pv;

    // 총알 데이터 수신용 캐시
    private List<BulletType> syncedBullets = new();

    private Coroutine hideCoroutine; // 코루틴 캐시용

    private void Start()
    {
        if (Manager.Game != null)
        {
            Manager.Game.OnTurnStart += OnTurnStarted;
        }

        if (bulletInfoPanel != null)
            bulletInfoPanel.SetActive(false);
    }

    private void OnTurnStarted()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SyncBulletsToAllClients();  // 마스터가 총알을 보내줌
        }
    }

    private void SyncBulletsToAllClients()
    {
        if (GunManager.Instance == null) return;

        List<BulletType> allBullets = new() { GunManager.Instance.LoadedBullet };
        allBullets.AddRange(GunManager.Instance.Magazine);

        // 실탄 → 공포탄 순으로 정렬
        // allBullets.Sort((a, b) => ((int)b).CompareTo((int)a));

        int[] serialized = allBullets.ConvertAll(b => (int)b).ToArray();
        pv.RPC(nameof(RPC_ReceiveBulletInfo), RpcTarget.All, serialized);
    }

    [PunRPC]
    public void RPC_ReceiveBulletInfo(int[] bulletArray)
    {
        syncedBullets.Clear();
        foreach (int i in bulletArray)
        {
            syncedBullets.Add((BulletType)i);
        }

        DisplayBulletsFromSyncedData();
    }

    public void DisplayBulletsFromSyncedData()
    {
        ClearBullets();

        float startX = -(syncedBullets.Count - 1) * spacing / 2f;
        int liveCount = 0, blankCount = 0;

        for (int i = 0; i < syncedBullets.Count; i++)
        {
            BulletType type = syncedBullets[i];
            GameObject prefab = (type == BulletType.live) ? liveBulletPrefab : blankBulletPrefab;
            if (type == BulletType.live) liveCount++;
            else if (type == BulletType.blank) blankCount++;

            Vector3 position = anchorPoint.position + anchorPoint.right * (startX + i * spacing) + offset;
            Quaternion rotation = anchorPoint.rotation;

            GameObject bullet = Instantiate(prefab, position, rotation, anchorPoint);
            spawnedBullets.Add(bullet);
        }
        Debug.Log("[DisplayBullets] 표시할 탄 배열: " + string.Join(", ", syncedBullets.Select(b => b.ToString())));

        if (bulletInfoPanel != null && bulletCountText != null)
        {
            bulletCountText.text = $"<color=red>실탄:</color> {liveCount}  /  <color=blue>공포탄:</color> {blankCount}";
            ShowBulletInfoPanel();
        }
    }

    
    //카메라 연출 추가
    private void ShowBulletInfoPanel()
    {
        //bulletinfo panel을 보여주는 vcam 활성화
        
        
        bulletInfoPanel.SetActive(true);
        bulletInfoPanel.transform.localScale = Vector3.zero;

        // 등장 연출
        bulletInfoPanel.transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack);

        // 기존 코루틴이 있으면 중단
        if (hideCoroutine != null)
        {
            StopCoroutine(hideCoroutine);
        }
        // 새로운 코루틴 시작
        hideCoroutine = StartCoroutine(HideBulletInfoPanelAfterDelay());
    }

    private IEnumerator HideBulletInfoPanelAfterDelay()
    {
        yield return new WaitForSeconds(showDuration);

        // 퇴장 연출
        bulletInfoPanel.transform.DOScale(0f, fadeDuration)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                bulletInfoPanel.SetActive(false);
              
            });

        hideCoroutine = null;
    }

    public void ClearBullets()
    {
        foreach (var bullet in spawnedBullets)
        {
            if (bullet != null)
                Destroy(bullet);
        }
        spawnedBullets.Clear();
    }

    private void OnDestroy()
    {
        if (Manager.Game != null)
        {
            Manager.Game.OnTurnStart -= OnTurnStarted;
        }
    }
}
