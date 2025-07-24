using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using LTH; // 임시 Playerm ItemData 정의용 네임스페이스
using DesignPattern; 
using DG.Tweening;

/// <summary>
/// 턴 종료 시 등장하는 보상 상자 매니저
/// 클릭 시 상자가 열리며, 아이템이 자동으로 빈 슬롯에 배치됨
/// </summary>

public class ItemBoxManager : Singleton<ItemBoxManager>
{
    [Header("보상 아이템 설정")]
    [SerializeField] private List<ItemData> newItems; // 등장 가능한 아이템 목록
    [SerializeField] private int newItemCount;

    [Header("상자 오브젝트")]
    [SerializeField] private Transform containerTransform;  // 서랍식으로 열릴 부분
    [SerializeField] private GameObject itemBoxPrefabs;     // 상자 전체 오브젝트
    [SerializeField] private Renderer[] boxRenderers;       // 머티리얼 투명도 조절용

    [Header("슬롯 연결")]
    [SerializeField] private DeskUI deskUI;

    private bool isOpened = false;
    private Material[] boxMaterials;

    private void Awake()
    {
        SingletonInit();

        // 각 Renderer의 머티리얼 인스턴스화
        boxMaterials = new Material[boxRenderers.Length];
        for (int i = 0; i < boxRenderers.Length; i++)
        {
            boxMaterials[i] = boxRenderers[i].material;
        }

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

        FadeInBox(); // 서서히 등장
    }

    private void FadeInBox()
    {
        for (int i = 0; i < boxMaterials.Length; i++)
        {
            float delay = i * 0.1f;
            boxMaterials[i].DOFade(1f, 0.5f).SetDelay(delay);
        }
    }

    private void SetBoxAlpha(float alpha)
    {
        foreach (var mat in boxMaterials)
        {
            var color = mat.color;
            color.a = alpha;
            mat.color = color;
        }
    }


    /// <summary>
    /// 클릭 시 상자 열리고 보상 아이템 지급
    /// </summary>
    public void OnBoxClicked()
    {
        if (isOpened) return;
        isOpened = true;

        // 서랍식 애니메이션
        containerTransform.DOLocalMoveZ(-0.03f, 0).SetEase(Ease.OutBack);

        // 아이템 지급
        var rewards = PickRandomRewards();
        AutoPlaceToSlots(rewards);

        // 잠시 후 상자 숨기기
        StartCoroutine(HideBoxAfterDelay());
    }

    /// <summary>
    /// 상자를 닫힌 위치로 되돌림
    /// </summary>
    private void CloseBoxImmediately()
    {
        containerTransform.localPosition = Vector3.zero;
        itemBoxPrefabs.SetActive(true);
    }

    /// <summary>
    /// 아이템을 랜덤하게 선택
    /// </summary>
    private List<ItemData> PickRandomRewards()
    {
        var result = new List<ItemData>();

        for (int i = 0; i < newItemCount; i++)
        {
            int rand = Random.Range(0, newItems.Count);
            result.Add(newItems[rand]); // 중복 허용
        }

        return result;
    }

    /// <summary>
    /// 빈 슬롯에 아이템 자동 배치
    /// </summary>
    private void AutoPlaceToSlots(List<ItemData> rewards)
    {
        var emptySlots = deskUI.GetEmptySlots();
        for (int i = 0; i < rewards.Count && i < emptySlots.Count; i++)
        {
            emptySlots[i].PlaceItem(rewards[i].itemPrefab);
        }
    }

    /// <summary>
    /// 연출 후 상자 숨김
    /// </summary>
    private IEnumerator HideBoxAfterDelay()
    {
        yield return new WaitForSeconds(1.5f);
        itemBoxPrefabs.SetActive(false);
        SetBoxAlpha(0f);
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 테스트용: 외부에서 아이템 직접 지정하여 상자 호출
    /// </summary>
    public void ShowBoxWithCustomItems(List<ItemData> customList)
    {
        newItems = customList;
        ShowBox();
    }
}