using Managers;
using Photon.Pun;
using UnityEngine;

public class GameStarter : MonoBehaviour
{
    private bool started = false;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("[GameStarter] 마스터 클라이언트 → InGameManager.StartGame 호출");
            StartGame();
        }
        else
        {
            Debug.Log("[GameStarter] 일반 클라이언트, 게임 시작 대기");
        }
    }

    private void StartGame()
    {
        if (started) return;
        started = true;

        if (InGameManager.Instance != null)
        {
            InGameManager.Instance.StartGame();
        }
        else
        {
            Debug.LogWarning("[GameStarter] InGameManager 인스턴스를 찾을 수 없습니다!");
        }
    }
}
