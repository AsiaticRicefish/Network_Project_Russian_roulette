using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHPUI : MonoBehaviour
{
    [Header("닉네임 & 체력 하트 UI")]
    [SerializeField] private TextMeshProUGUI myNicknameText;
    [SerializeField] private Transform myHPPanel;
    [SerializeField] private TextMeshProUGUI enemyNicknameText;
    [SerializeField] private Transform enemyHPPanel;
    [SerializeField] private GameObject heartPrefab;

    [Header("게임 오버 UI")]
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;

    private GamePlayer myPlayer;
    private string myId;
    private bool hasShownGameOver = false;
    private Dictionary<string, GamePlayer> players;

    private void Start()
    {
        myId = PhotonNetwork.NickName;
        StartCoroutine(WaitAndBind());
    }

    private IEnumerator WaitAndBind()
    {
        float timeout = 2f;
        float timer = 0f;

        while (PlayerManager.Instance == null || PlayerManager.Instance.GetAllPlayers().Count == 0)
        {
            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
            if (timer > timeout) yield break;
        }

        players = PlayerManager.Instance.GetAllPlayers();
        myPlayer = FindPlayerByNickname_UIOnly(myId);

        foreach (var player in players.Values)
        {
            player.OnHpChanged += UpdateHpUIForAll;
        }

        UpdateHpUIForAll();
    }

    private void OnDestroy()
    {
        if (players != null)
        {
            foreach (var player in players.Values)
            {
                player.OnHpChanged -= UpdateHpUIForAll;
            }
        }
    }

    private void UpdateHpUIForAll(int _, bool __)
    {
        UpdateHpUIForAll();
    }

    private void UpdateHpUIForAll()
    {
        foreach (var p in players.Values)
        {
            if (p.MaxHp <= 0)
                return;
        }

        foreach (var player in players.Values)
        {
            bool isMine = player.Nickname == myId;

            if (isMine)
            {
                myNicknameText.text = player.Nickname;
                UpdateHeartUI(myHPPanel, player.CurrentHp, player.MaxHp, true);

                if (!player.IsAlive && !hasShownGameOver)
                {
                    hasShownGameOver = true;
                    ShowGameOverUI();
                }
            }
            else
            {
                enemyNicknameText.text = player.Nickname;
                UpdateHeartUI(enemyHPPanel, player.CurrentHp, player.MaxHp, false);
            }
        }
        
        // 닉네임을 승자이름으로 전달하기 위함
        if (myPlayer != null && myPlayer.IsAlive && !hasShownGameOver)
        {
            foreach (var pair in players)
            {
                if (pair.Key != myId && !pair.Value.IsAlive)
                {
                    hasShownGameOver = true;
                    ShowGameOverUI();
                    break;
                }
            }
        }
    }

    private void UpdateHeartUI(Transform panel, int currentHp, int maxHp, bool isMine)
    {
        // 하트가 이미 생성된 경우 재사용
        if (panel.childCount != maxHp)
        {
            // 모두 제거 후 재생성
            foreach (Transform child in panel)
            {
                Destroy(child.gameObject);
            }

            for (int i = 0; i < maxHp; i++)
            {
                GameObject heart = Instantiate(heartPrefab, panel);
                Image img = heart.GetComponent<Image>();
                img.color = isMine ? Color.red : new Color(0.5f, 0.7f, 1f);
            }
        }

        // 하트 활성/비활성만 조정
        for (int i = 0; i < maxHp; i++)
        {
            var heart = panel.GetChild(i).GetComponent<Image>();
            heart.enabled = i < currentHp; // 현재 체력 이하만 표시
        }
    }

    private void ShowGameOverUI()
    {
        string winner = FindAlivePlayerName();
        if (GameOverSync.Instance != null)
        {
            GameOverSync.Instance.GameOver(winner);
        }
    }

    private string FindAlivePlayerName()
    {
        foreach (var pair in players)
        {
            if (pair.Value.IsAlive)
                return pair.Value.Nickname;
        }
        return "상대방";
    }

    private GamePlayer FindPlayerByNickname_UIOnly(string nickname)
    {
        foreach (var pair in PlayerManager.Instance.GetAllPlayers())
        {
            if (pair.Value.Nickname == nickname)
                return pair.Value;
        }
        return null;
    }
}