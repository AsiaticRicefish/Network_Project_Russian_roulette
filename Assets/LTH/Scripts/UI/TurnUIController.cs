using DG.Tweening;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Utils;

public class TurnUIController : MonoBehaviour
{
    [SerializeField] private TMP_Text turnMessageText;
    [SerializeField] private CanvasGroup canvasGroup;

    private string lastTurnId = "";

    private void Update()
    {
        string currentId = TurnSync.CurrentTurnPlayerId;
       // Debug.Log($"[UI] 현재 감지된 턴 ID: {currentId}");
        
        // 턴 ID가 바뀌었을 때만 처리
        if (!string.IsNullOrEmpty(currentId) && currentId != lastTurnId)
        {
            Debug.Log($"[UI] 턴 변경 감지: {lastTurnId} → {currentId}");
            lastTurnId = currentId;
            ShowMessageForTurn(currentId);
        }
    }

    private void ShowMessageForTurn(string playerId)
    {
        string myId = PhotonNetwork.LocalPlayer.NickName;
        string message = playerId == myId
                ? "당신의 턴입니다"
            : $"상대방의 턴입니다";

        ShowTurnMessage(message);
    }

    private void ShowTurnMessage(string message)
    {
        turnMessageText.text = message;
        canvasGroup.alpha = 0;
        canvasGroup.DOFade(1f, 0.5f).OnComplete(() =>
        {
            DOVirtual.DelayedCall(2f, () =>
            {
                canvasGroup.DOFade(0f, 0.5f);
            });
        });
    }
}