using Managers;
using Michsky.UI.ModernUIPack;
using Photon.Pun;
using Photon.Realtime;
using Sound;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class GameOverSync : MonoBehaviourPunCallbacks
{
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI winnerText;
    [SerializeField] private TextMeshProUGUI countdownText;
    [SerializeField] private BgmPlayer bgmPlayer;
    
    private bool hasShow = false;
    private bool isGameOver = false;

    public static GameOverSync Instance;

    private ModalWindowManager gameOverPanelManager;
    
    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        gameOverPanelManager = gameOverPanel.GetComponent<ModalWindowManager>();
        InGameManager.Instance.OnGameEnd += () =>
        {
            GameOver(FindWinnerName());
        };
    }


    public string FindWinnerName()
    {
        foreach (var player in PlayerManager.Instance.GetAllPlayers().Values)
        {
            if (player.IsAlive)
                return player.Nickname;
        }
        return "상대방";
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
        
        //bgm 변경
        bgmPlayer.PlayBgm(1);

        //닉네임 파싱
        winnerNickname = Util_LDH.GetUserNickname(winnerNickname);
        
        
        if (gameOverPanel != null)
            gameOverPanelManager.OpenWindow();

        if (winnerText != null)
            winnerText.text = $"{winnerNickname}is the winner!!";
       
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
                countdownText.text = $"Returning to the lobby in {Mathf.CeilToInt(remaining)} seconds...";

            yield return new WaitForSeconds(1f);
            remaining -= 1f;
        }
        
        // photonView.RPC("GoToLobbyScene", RpcTarget.All);
        GoToLobbyScene();
    }
    public override void OnLeftRoom()
    {
        //if (Managers.Manager.manager == null)
        //{
        //    Debug.LogWarning("@Manager가 없어서 재생성 시도");

        //    Managers.Manager.Initialize();
        //}
        
        //인게임 매니저 release
        Util_LDH.ReleaseInGameManager();

        //플레이어 커스텀 프로퍼티 초기화
        Util_LDH.ClearAllPlayerProperty();
        
        Debug.Log("로비로 이동합니다.");
        SceneManager.LoadScene("Lobby");
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Debug.Log($"플레이어 {otherPlayer.NickName}가 나감");

        if (!hasShow)
        {
            //다른 플레이어가 나갔을 때, 게임 오버 상태가 아니고 게임이 진행 중이라면
            //로컬 플레이어도 나가게 처리한다.
            PhotonNetwork.LeaveRoom(false);
        }
    }
}
