using DG.Tweening;
using Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Photon.Pun;

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

        if (bulletInfoPanel != null && bulletCountText != null)
        {
            bulletCountText.text = $"<color=red>실탄:</color> {liveCount}  /  <color=blue>공포탄:</color> {blankCount}";
            ShowBulletInfoPanel();
        }
    }

    private void ShowBulletInfoPanel()
    {
        bulletInfoPanel.SetActive(true);
        bulletInfoPanel.transform.localScale = Vector3.zero;

        // 등장 연출
        bulletInfoPanel.transform.DOScale(1f, fadeDuration).SetEase(Ease.OutBack);

        // 일정 시간 후 퇴장
        StartCoroutine(HideBulletInfoPanelAfterDelay());
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
