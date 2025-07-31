using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverSync : MonoBehaviourPun
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;

    private bool hasShown = false;

    public static GameOverSync Instance;

    private void Awake()
    {
        Instance = this;
    }

    /// <summary>
    /// 외부에서 승자 이름을 전달해서 모든 클라이언트에 패널 띄움
    /// </summary>
    public void GameOver(string winnerNickname)
    {
        if (!hasShown)
        {
            photonView.RPC("ShowGameOverPanel", RpcTarget.All, winnerNickname);
        }
    }

    [PunRPC]
    private void ShowGameOverPanel(string winnerNickname)
    {
        if (hasShown) return;

        hasShown = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (winnerText != null)
            winnerText.text = $"{winnerNickname}님이 우승했습니다!";

        Debug.Log($" 모든 클라이언트에서 게임 종료 UI 표시. 승자: {winnerNickname}");
    }
}
