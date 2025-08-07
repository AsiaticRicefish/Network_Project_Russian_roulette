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

        int liveCount = syncedBullets.Count(b => b == BulletType.live);
        int blankCount = syncedBullets.Count(b => b == BulletType.blank);

        float totalCount = liveCount + blankCount;
        float startX = -(totalCount - 1) * spacing / 2f;
        int index = 0;

        // 실탄을 먼저 배치 

        for (int i = 0; i < liveCount; i++, index++)
        {
            Vector3 position = anchorPoint.position + anchorPoint.right * (startX + index * spacing) + offset;
            GameObject bullet = Instantiate(liveBulletPrefab, position, anchorPoint.rotation, anchorPoint);
            spawnedBullets.Add(bullet);
        }

        // 공포탄 배치
        for (int i = 0; i < blankCount; i++, index++)
        {
            Vector3 position = anchorPoint.position + anchorPoint.right * (startX + index * spacing) + offset;
            GameObject bullet = Instantiate(blankBulletPrefab, position, anchorPoint.rotation, anchorPoint);
            spawnedBullets.Add(bullet);
        } 

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
