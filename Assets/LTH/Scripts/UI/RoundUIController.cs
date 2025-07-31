using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RoundUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text roundText;

    private void OnEnable()
    {
        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.OnRoundCountChange += UpdateRoundText;
            UpdateRoundText(); // 초기 1라운드도 표시
        }
    }

    private void OnDisable()
    {
        try
        {
            InGameManager.Instance.OnRoundCountChange -= UpdateRoundText;
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[RoundUIController] OnDisable 예외: " + e.Message);
        }
    }

    private void UpdateRoundText()
    {
        int current = InGameManager.Instance.CurrentRound;
        int total = InGameManager.Instance.TotalRound;
        roundText.text = $"라운드 {current} / {total}";
    }
}