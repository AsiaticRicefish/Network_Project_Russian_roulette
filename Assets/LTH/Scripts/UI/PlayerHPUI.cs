using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

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
        
        //인게임 로직에서 게임 오버가 됐을 때 게임 오버를 보여주기 위해 이벤트 구독으로 변경
        InGameManager.Instance.OnGameEnd += ShowGameOverUI;
    }

    //----- 생략----- //
    //Start 부분에 있는 StartCoroutine은 제거
    //PlayerManager의 OnAddPlayer는 _players 딕셔너리에 Player가 추가될때마다 호출되는데,
    //해당 딕셔너리에 플레이어가 추가되는 시점은 : 모두 씬에 들어왔을 때 각자 자기의 플레이어 오브젝트를 photon instantiate 하면 -> PlayerController에서 딕셔너리에 추가한다.
    //그러면 PlayerHPUI는 OnAddPlayer 이벤트에 WaitandBind 대신에, Bind라는 함수를 구독시킨다.
    //Bind 함수는 PlayerManager.Instance.GetAllPlayers == 2개됐을 때 -> ui랑 연동하는 부분 로직을 그대로 실행

    private void OnEnable()
    {
        PlayerManager.Instance.OnAddPlayer += Bind;
    }

    // 2는 현재 플레이어 총인원
    private void Bind()
    {
        if (PlayerManager.Instance.GetAllPlayers().Count != 2) return;

        //Debug.Log($"[PlayerHPUI]에서 Bind 호출됨"); - 호출되는거 확인
        //자기 UI만 뛰우고 있다.
        players = PlayerManager.Instance.GetAllPlayers();

        myPlayer = FindPlayerByNickname_UIOnly(myId);

        foreach (var player in players.Values)
        {
            player.OnHpChanged += UpdateHpUIForAll;
        }

        UpdateHpUIForAll();
    }

    #region 이전코드 IEnumerator WaitAndBind()
    //private IEnumerator WaitAndBind()
    //{
    //    float timeout = 2f;
    //    float timer = 0f;

    //    while (PlayerManager.Instance == null || PlayerManager.Instance.GetAllPlayers().Count == 0)
    //    {
    //        yield return new WaitForSeconds(0.1f);
    //        timer += 0.1f;
    //        if (timer > timeout) yield break;
    //    }

    //    players = PlayerManager.Instance.GetAllPlayers();
    //    myPlayer = FindPlayerByNickname_UIOnly(myId);

    //    foreach (var player in players.Values)
    //    {
    //        player.OnHpChanged += UpdateHpUIForAll;
    //    }

    //    UpdateHpUIForAll();
    //}
    #endregion

    private void OnDestroy()
    {
        if (players != null)
        {
            foreach (var player in players.Values)
            {
                player.OnHpChanged -= UpdateHpUIForAll;
            }
        }
        //PlayerManager.Instance.OnAddPlayer -= Bind;
    }

    private void UpdateHpUIForAll(int _, bool __)
    {
        UpdateHpUIForAll();
    }

    private void UpdateHpUIForAll()
    {
        foreach (var player in players.Values)
        {
            if (player.MaxHp <= 0)
                return; // 아직 게임 시작 전이거나 초기화 안 됨
        }
        foreach (var player in players.Values)
        {
            bool isMine = player.Nickname == myId;

            if (isMine)
            {
                myNicknameText.text = Util_LDH.GetUserNickname(player.Nickname);
                UpdateHeartUI(myHPPanel, player.CurrentHp, player.MaxHp, true);

                if (!player.IsAlive && !hasShownGameOver)
                {
                    hasShownGameOver = true;
                   // ShowGameOverUI();         //인게임 로직에서 게임 오버가 됐을 때 게임 오버를 보여주기 위해 이벤트 구독으로 변경
                }
            }
            else
            {
                enemyNicknameText.text = Util_LDH.GetUserNickname(player.Nickname);
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
                    // ShowGameOverUI();        //인게임 로직에서 게임 오버가 됐을 때 게임 오버를 보여주기 위해 이벤트 구독으로 변경
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