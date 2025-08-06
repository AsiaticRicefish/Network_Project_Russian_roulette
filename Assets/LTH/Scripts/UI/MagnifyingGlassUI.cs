using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MagnifyingGlassUI : MonoBehaviour
{
    [Header("UI 연결")]
    [SerializeField] private GameObject magnifyingPanel;
    [SerializeField] private Image bulletImage;

    [Header("총알 이미지")]
    [SerializeField] private Sprite liveBulletSprite;
    [SerializeField] private Sprite blankBulletSprite;

    public void ShowBulletInfo(BulletType type)
    {
        magnifyingPanel.SetActive(true);

        bulletImage.sprite = (type == BulletType.live) ? liveBulletSprite : blankBulletSprite;

        magnifyingPanel.transform.DOShakePosition(0.5f, new Vector3(10f, 0f, 0f));

        Invoke(nameof(HidePanel), 3f);
    }

    private void HidePanel()
    {
        magnifyingPanel.SetActive(false);
    }
}
