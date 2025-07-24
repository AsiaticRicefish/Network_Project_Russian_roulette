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
    [Header("TurnEnd NewItem")]
    [SerializeField] private List<ItemData> newItems; // 등장 가능한 아이템 목록
    [SerializeField] private int newItemCount;

    [Header("ItemBox Object")]
    [SerializeField] private GameObject itemBoxPrefabs;     // 상자 전체 오브젝트
    [SerializeField] private Renderer[] boxRenderers;       // 머티리얼 투명도 조절용
    [SerializeField] private Transform lidTransform;        // 상자 뚜껑 (CommonMediumLid)
    [SerializeField] private Vector3 openRotation = new Vector3(-120f, 0f, 0f); // 뚜껑 열리는 각도

    [Header("Connect Slot")]
    [SerializeField] private DeskUI deskUI;

    private bool isOpened = false;
    private Material[] boxMaterials;

    private void Awake()
    {
        SingletonInit();

        InitMaterials();

        CloseBoxImmediately(); // 시작 시 닫힌 상태로 초기화
    }

    private void InitMaterials()
    {
        boxMaterials = new Material[boxRenderers.Length];
        for (int i = 0; i < boxRenderers.Length; i++)
        {
            boxMaterials[i] = boxRenderers[i].material;

            // 내부는 뒤쪽에 렌더링되도록 설정
            if (boxMaterials[i].name.ToLower().Contains("container"))
                boxMaterials[i].renderQueue = 3100;
            else
                boxMaterials[i].renderQueue = 3000;

            // 초기 알파값 0
            var color = boxMaterials[i].color;
            color.a = 0f;
            boxMaterials[i].color = color;
        }
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
            float delay = i * 0.3f;
            boxMaterials[i].DOFade(1f, 1.2f)
                          .SetDelay(delay)
                          .SetEase(Ease.OutQuad);
        }
    }

    /// <summary>
    /// 상자를 닫힌 상태로 초기화
    /// </summary>
    private void CloseBoxImmediately()
    {
        lidTransform.localRotation = Quaternion.identity; // 뚜껑 닫기
        itemBoxPrefabs.SetActive(true);
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

        lidTransform.DOLocalRotate(openRotation, 0.7f)
                 .SetEase(Ease.OutBack)
                 .OnComplete(() =>
                 {
                     var rewards = PickRandomRewards();
                     AutoPlaceToSlots(rewards);
                     StartCoroutine(HideBoxAfterDelay());
                 });
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