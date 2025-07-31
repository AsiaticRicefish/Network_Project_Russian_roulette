using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI hpText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;

    private GamePlayer myPlayer;
    private bool hasShownGameOver = false;

    private void Start()
    {
        StartCoroutine(WaitAndBind());
    }

    private IEnumerator WaitAndBind()
    {
        string myNickname = PhotonNetwork.NickName;

        float timeout = 2f;
        float timer = 0f;

        while (myPlayer == null && timer < timeout)
        {
            myPlayer = FindPlayerByNickname_UIOnly(myNickname);
            if (myPlayer != null) break;

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }

        if (myPlayer != null)
        {
            myPlayer.OnHpChanged += UpdateHpUI;
            UpdateHpUI(myPlayer.CurrentHp, myPlayer.IsAlive);
        }
    }

    private void OnDestroy()
    {
        if (myPlayer != null)
            myPlayer.OnHpChanged -= UpdateHpUI;
    }

    private void UpdateHpUI(int currentHp, bool isAlive)
    {
        string state = isAlive ? "<color=green>생존</color>" : "<color=red>사망</color>";
        hpText.text = $"HP: {currentHp} / {myPlayer.MaxHp}   {state}";

        // 사망 처리
        if (!isAlive && !hasShownGameOver)
        {
            hasShownGameOver = true;
            ShowGameOverUI();
        }
    }

    private void ShowGameOverUI()
    {
        string winner = FindAlivePlayerName();
        if (winnerText != null)
        {
            winnerText.text = $"{winner}님이 우승했습니다!";
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        Debug.Log($"[PlayerHPUI] 게임 종료 UI 표시됨. 우승자: {winner}");
    }

    private string FindAlivePlayerName()
    {
        var players = PlayerManager.Instance.GetAllPlayers();
        foreach (var pair in players)
        {
            if (pair.Value.IsAlive && pair.Value.Nickname != PhotonNetwork.NickName)
            {
                return pair.Value.Nickname;
            }
        }
        return "상대방";
    }

    private GamePlayer FindPlayerByNickname_UIOnly(string nickname)
    {
        var players = PlayerManager.Instance.GetAllPlayers();
        foreach (var pair in players)
        {
            if (pair.Value.Nickname == nickname)
                return pair.Value;
        }
        return null;
    }
}
