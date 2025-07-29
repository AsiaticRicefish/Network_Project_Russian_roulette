using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Managers;
using System;

public class FireSync : MonoBehaviourPun
{
    [SerializeField] private TurnSync turnSync;
    private string myId;
    private void Start()
    {
        myId = PhotonNetwork.IsMasterClient ? "player1" : "player2";

        // 마스터 클라이언트가 게임 시작 시 탄을 장전하고 전체에 동기화
        if (PhotonNetwork.IsMasterClient)
        {
            GunManager.Instance.SetLoadedBullet(default); // 현재 탄 초기화
            GunManager.Instance.Magazine.Clear();         // 탄창 비우기
            GunManager.Instance.Reload();                 // 새로운 탄 장전

            // 탄 정보 리스트 생성: 첫 번째는 장전된 탄, 이후는 탄창
            var current = GunManager.Instance.Magazine.ToArray();
            var bullets = new List<int> { (int)GunManager.Instance.LoadedBullet };
            bullets.AddRange(Array.ConvertAll(current, b => (int)b));

            photonView.RPC("ReloadSync", RpcTarget.All, bullets.ToArray(), bullets[0]);
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            BulletType fireBullet = GunManager.Instance.LoadedBullet;
            // 발사 동기화: 모든 클라이언트에게 발사 정보 전달
            photonView.RPC("Fire", RpcTarget.All, myId, (int)fireBullet);
            turnSync.photonView.RPC("RequestEndTurn", RpcTarget.MasterClient);
        }
    }

    // 탄창 전체를 클라이언트와 동기화하는 함수
    private void SendReloadSync()
    {
        BulletType[] currentMagazine = GunManager.Instance.Magazine.ToArray();

        List<int> totalBullets = new();
        totalBullets.Add((int)GunManager.Instance.LoadedBullet); // 첫 탄
        foreach (BulletType bullet in currentMagazine)
        {
            totalBullets.Add((int)bullet); // 나머지 탄
        }

        photonView.RPC("ReloadSync", RpcTarget.All, totalBullets.ToArray(), totalBullets[0]);
    }

    // 모든 클라이언트에서 발사 동작을 실행하는 RPC
    [PunRPC]
    private void Fire(string playerId, int bulletInt)
    {
        BulletType bullet = (BulletType)bulletInt;
        string targetId = (playerId == "player1") ? "player2" : "player1";
        Debug.LogError($"{playerId}이(가) {bullet} 탄을 발사했습니다.");
        if (bullet == BulletType.live)
        {
            Debug.LogError($"{targetId}이 데미지를 입었습니다.");
        }
        else
        {
            Debug.LogError($"{targetId}이 데미지를 입지 않았습니다.");
        }

        // 탄 소모 및 다음 탄 장전
        GunManager.Instance.IsEnhanced = false;
        if (!GunManager.Instance.Magazine.TryDequeue(out var next))
        {
            next = default;
        }
        GunManager.Instance.SetLoadedBullet(next);
        Debug.LogError($"장전된 탄: {GunManager.Instance.LoadedBullet}, 남은 탄 수: {GunManager.Instance.Magazine.Count}");

        // 탄을 모두 소진했을 경우 마스터만 자동 장전 후 동기화
        if (PhotonNetwork.IsMasterClient && GunManager.Instance.LoadedBullet == default && GunManager.Instance.Magazine.Count == 0)
        {
            GunManager.Instance.Reload();

            BulletType[] current = GunManager.Instance.Magazine.ToArray();
            List<int> bullets = new List<int> { (int)GunManager.Instance.LoadedBullet };
            bullets.AddRange(Array.ConvertAll(current, b => (int)b));

            photonView.RPC("ReloadSync", RpcTarget.All, bullets.ToArray(), bullets[0]);
        }

    }

    // 마스터에서 생성된 탄창을 클라이언트에게 동기화
    [PunRPC]
    private void ReloadSync(int[] bullets, int loadedBullet)
    {
        GunManager.Instance.Magazine.Clear();
        GunManager.Instance.SetLoadedBullet((BulletType)loadedBullet);

        for (int i = 1; i < bullets.Length; i++)
        {
            GunManager.Instance.Magazine.Enqueue((BulletType)bullets[i]);
        }

        int liveCount = 0;
        int blankCount = 0;
        foreach (BulletType bullet in GunManager.Instance.Magazine)
        {
            if (bullet == BulletType.live) liveCount++;
            else if (bullet == BulletType.blank) blankCount++;
        }
        Debug.LogWarning($"장전된 탄: {GunManager.Instance.LoadedBullet}, 남은 탄 수: {GunManager.Instance.Magazine.Count}, 실탄: {liveCount}, 공포탄: {blankCount}");
    }
}
