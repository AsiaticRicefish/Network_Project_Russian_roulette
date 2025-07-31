using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnSync : MonoBehaviourPun
{
    // 현재 턴 플레이어의 ID를 모든 클라이언트에서 참조
    public static string CurrentTurnPlayerId { get; private set; } = "";

    private string myId;

    private void Start()
    {
        myId = PhotonNetwork.LocalPlayer.NickName;

        // InGameManager의 턴 이벤트 구독
        InGameManager.Instance.OnTurnStart += OnTurnStart;
        InGameManager.Instance.OnTurnEnd += OnTurnEnd;
        InGameManager.Instance.StartGame();
    }

    private void OnTurnStart()
    {
        // 마스터클라이언트만 턴 결정하고 클라이언트한테 전달
        if (PhotonNetwork.IsMasterClient)
        {
            string turnId = InGameManager.Instance.CurrentTurn;
            photonView.RPC("SyncTurn", RpcTarget.All, turnId);
        }
    }

    // 턴 종료
    private void OnTurnEnd()
    {
        CurrentTurnPlayerId = "";
    }


    [PunRPC]
    private void SyncTurn(string playerId)
    {
        CurrentTurnPlayerId = playerId;
        Debug.Log($"현재 턴 플레이어: {playerId}");
    }

    // 클라이언트에서 호출되는 턴 종료 요청 RPC
    [PunRPC]
    private void RequestEndTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;

        InGameManager.Instance.EndTurn(); // 턴을 마스터가 넘기고

        // 다음 턴 플레이어를 직접 Sync
        string nextPlayerId = InGameManager.Instance.CurrentTurn;
        photonView.RPC("SyncTurn", RpcTarget.All, nextPlayerId); // 턴 동기화
    }
}
