using DG.Tweening;
using LDH_Animation;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotEffectController : MonoBehaviour
{
    [Header("Transform Targets")]
    [SerializeField] private Transform slotTransform;
    [SerializeField] private CanvasGroup canvasGroup;

    [Header("Effect Prefabs")]
   // [SerializeField] private GameObject clickEffectPrefab;
    [SerializeField] private GameObject useEffectPrefab;
    [SerializeField] private GameObject itemAppearEffectPrefab;

    [Header("Animation Settings")]
    [SerializeField] private float appearMoveHeight = 0.3f;
    [SerializeField] private float appearDuration = 0.4f;
    [SerializeField] private float shakeDuration = 0.2f;
    [SerializeField] private float shakeStrength = 8f;

    private void Reset()
    {
        if (slotTransform == null)
            slotTransform = transform;

        if (canvasGroup == null)
            canvasGroup = GetComponent<CanvasGroup>();
    }

    /// <summary>
    /// 슬롯 전체가 등장할 때 사용
    /// </summary>
    public void PlayAppearEffect()
    {
        if (slotTransform == null || canvasGroup == null)
        {
            return;
        }

        Vector3 startPos = slotTransform.position - new Vector3(0, appearMoveHeight, 0);
        canvasGroup.alpha = 0;
        slotTransform.position = startPos;

        new AnimationBuilder(slotTransform)
            .MoveTo(startPos + new Vector3(0, appearMoveHeight, 0), appearDuration)
            .FadeIn(canvasGroup, 0.3f)
            .Shake(shakeDuration, shakeStrength)
            .Play();
    }


    /// <summary>
    ///  아이템 생성되는 경우 나오는 연출
    /// </summary>
    public void PlayItemAppearEffect(Vector3 position)
    {
        if (itemAppearEffectPrefab != null)
        {
            GameObject fx = Instantiate(itemAppearEffectPrefab, position, Quaternion.identity);
            fx.transform.SetParent(transform, worldPositionStays: true);
            Destroy(fx, 1.5f);
        }
    }


    /// <summary>
    /// 아이템이 사용되어 사라질 때 실행되는 연출
    /// </summary>
    public void PlayUseEffect(GameObject currentItem)
    {
        if (currentItem == null) return;

        if (useEffectPrefab != null)
        {
            GameObject fx = Instantiate(useEffectPrefab, currentItem.transform.position, Quaternion.identity);
            Destroy(fx, 2f);
        }

        currentItem.transform.DOScale(Vector3.zero, 0.3f)
            .SetEase(Ease.InBack)
            .OnComplete(() => Destroy(currentItem));
    }
}
