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
    }

    private void OnTurnStart()
    {
        // 마스터클라이언트만 턴 결정하고 클라이언트한테 전달
        if (PhotonNetwork.IsMasterClient)
        {
            string turnId = CurrentTurnPlayerId;
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
        Debug.Log($"[TurnSync] 현재 턴 플레이어: {playerId}");
    }

    // 클라이언트에서 호출되는 턴 종료 요청 RPC
    [PunRPC]
    private void RequestEndTurn()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (InGameManager.Instance == null)
        {
            Debug.LogError("[TurnSync] EndTurn 실패: InGameManager.Instance가 null입니다.");
            return;
        }
        InGameManager.Instance.EndTurn();
    }
}
