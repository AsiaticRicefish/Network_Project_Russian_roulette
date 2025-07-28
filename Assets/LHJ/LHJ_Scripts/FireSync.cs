using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class FireSync : MonoBehaviourPun
{
    private string myId;

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
        }
        if (Input.GetKeyDown(KeyCode.R))
        {
            // 마스터 클라이언트만 탄창 장전
            if (PhotonNetwork.IsMasterClient)
            {
                GunManager.Instance.Reload();

                // 클라이언트에게 탄창 정보 전달
                List<int> bulletList = new();
                foreach (var bullet in GunManager.Instance.Magazine)
                {
                    bulletList.Add((int)bullet);
                }
                photonView.RPC("Reload", RpcTarget.All, bulletList.ToArray());
            }
        }
    }

    [PunRPC]
    private void Fire(string playerId, int bulletInt)
    {
        // 발사된 탄 종류를 enum으로 변환
        BulletType bullet = (BulletType)bulletInt;

        // 타겟 ID는 발사한 플레이어의 반대편으로 설정
        string targetId = (playerId == "player1") ? "player2" : "player1";
        Debug.LogError($"[발사] {playerId}이(가) {bullet} 탄을 발사했습니다.");

        if (bullet == BulletType.live)
            Debug.LogError($"{targetId}이 데미지를 입었습니다.");
        else
            Debug.LogError($"{targetId}이 데미지를 입지않았습니다.");
        // 발사 처리 타겟현재 null
        GunManager.Instance.Fire(null);

        //_loadedBullet 보호수준떄문에 주석처리
        //Debug.LogError($"장전된 탄: {GunManager.Instance._loadedBullet}, 남은 탄 수: {GunManager.Instance.Magazine.Count}");
        // InGameManager.Instance.EndTurn();     // 게임 매니저 EndTurn호출
    }
    [PunRPC]
    private void Reload(int[] bullets)
    {
        // 기존 탄창 초기화 
        GunManager.Instance.Magazine.Clear();
        
        // 배열로 탄창 구성
        foreach (int b in bullets)
        {
            GunManager.Instance.Magazine.Enqueue((BulletType)b);
        }
        //_loadedBullet 보호수준떄문에 주석처리
        //Debug.LogWarning($"남은 탄 수: {GunManager.Instance.Magazine.Count}, 첫 탄: {GunManager.Instance._loadedBullet}");
    }
}
