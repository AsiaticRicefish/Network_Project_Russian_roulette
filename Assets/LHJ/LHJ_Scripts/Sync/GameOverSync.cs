using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class GameOverSync : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI countdownText;

    private bool hasShow = false;

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
        if (!hasShow)
        {
            photonView.RPC("ShowGameOverPanel", RpcTarget.All, winnerNickname);
        }
    }

    [PunRPC]
    private void ShowGameOverPanel(string winnerNickname)
    {
        if (hasShow) return;

        hasShow = true;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        if (winnerText != null)
            winnerText.text = $"{winnerNickname}님이 우승했습니다!";

        Debug.Log($" 모든 클라이언트에서 게임 종료 UI 표시. 승자: {winnerNickname}");
        StartCoroutine(GoBackToLobbyAfterDelay(5f));
    }
    [PunRPC]
    private void GoToLobbyScene()
    {
        PhotonNetwork.LeaveRoom();
    }
    private IEnumerator GoBackToLobbyAfterDelay(float delay)
    {
        float remaining = delay;

        while (remaining > 0)
        {
            if (countdownText != null)
                countdownText.text = $"{Mathf.CeilToInt(remaining)}초 후 로비로 이동합니다...";

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }

        GunManager.Release();
        PlayerManager.Release();
        InGameManager.Release();
        ItemBoxSpawnerManager.Release();
        DeskUIManager.Release();

        photonView.RPC("GoToLobbyScene", RpcTarget.All);
    }
    public override void OnLeftRoom()
    {
        if (Managers.Manager.manager == null)
        {
            Debug.LogWarning("@Manager가 없어서 재생성 시도");

            Managers.Manager.Initialize();
        }
        PhotonNetwork.LoadLevel("LHJ_TestScene");
    }
}
