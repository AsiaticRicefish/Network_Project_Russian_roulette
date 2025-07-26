using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireSync : MonoBehaviourPun
{
    private string myId;
    private int fireCount = 0;   // 누적 발사 횟수(Test용)

    private void Start()
    {
        myId = PhotonNetwork.IsMasterClient ? "player1" : "player2";
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Todo: 현재 GunManager._loadedBullet가 private이라 참조 에러 발생 중 추후 코드 주석 해제 예정
            // 발사 직전 현재 장전된 탄을 가져옴
            // BulletType fireBullet = GunManager.Instance._loadedBullet;
            // 발사 동기화: 모든 클라이언트에게 발사 정보 전달
            // photonView.RPC("Fire", RpcTarget.All, myId, (int)fireBullet);
            // 발사 처리
            GunManager.Instance.Fire(null);
        }
    }

    [PunRPC]
    private void Fire(string playerId, int bulletInt)
    {
        // 발사된 탄 종류를 enum으로 변환
        BulletType bullet = (BulletType)bulletInt;

        // 타겟 ID는 발사한 플레이어의 반대편으로 설정
        string targetId = (playerId == "player1") ? "player2" : "player1";
        fireCount++;
        Debug.Log($"[발사] {playerId}이(가) {bullet} 탄을 발사했습니다.(누적 발사 횟수: {fireCount})");

        if (bullet == BulletType.live)
            Debug.Log($"{targetId}이 데미지를 입었습니다.");
        else
            Debug.Log($"{targetId}이 데미지를 입지않았습니다.");

        InGameManager.Instance.EndTurn();     // 게임 매니저 EndTurn호출
    }
}
