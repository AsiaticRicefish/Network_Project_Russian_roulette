using DG.Tweening;
using LDH_Animation;
using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlotEffectController : MonoBehaviour
{
    [Header("Effect Prefabs")]
    [SerializeField] private GameObject useEffectPrefab;
    [SerializeField] private GameObject itemAppearEffectPrefab;

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
            .SetEase(Ease.InBack);
    }
}
