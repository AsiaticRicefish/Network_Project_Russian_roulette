using Managers;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Utils;

public class FireSync : MonoBehaviourPun
{
    public static event Action<string, BulletType> OnPlayerHit;
    [SerializeField] private TurnSync turnSync;
    private string myId;
    private void Start()
    {
        myId = PhotonNetwork.NickName;
        InGameManager.Instance.OnGameStart += OnStartGame;

    }

    private void OnStartGame()
    {
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
            bullets.Sort((a, b) => b.CompareTo(a));

            photonView.RPC("ReloadSync", RpcTarget.All, bullets.ToArray(), bullets[0]);
        }
    }

    // private void Update()
    // {
    //     if (Input.GetKeyDown(KeyCode.Space))
    //     {
    //         if (TurnSync.CurrentTurnPlayerId != myId)
    //         {
    //             Debug.LogWarning($"[FireSync] 내 턴 아님 → 발사 차단 (myId: {myId}, 현재 턴: {TurnSync.CurrentTurnPlayerId})");
    //             return;
    //         }
    //
    //         // 내 턴이면 발사 진행
    //         BulletType fireBullet = GunManager.Instance.LoadedBullet;
    //         photonView.RPC("Fire", RpcTarget.All, myId, (int)fireBullet);
    //         turnSync.photonView.RPC("RequestEndTurn", RpcTarget.MasterClient);
    //     }
    // }

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


    /// <summary>
    /// 모든 클라이언트에서 발사 동작을 실행하는 RPC
    /// </summary>
    /// <param name="playerId">photon network player nickname</param>
    /// <param name="bulletInt"></param>
    [PunRPC]
    private void Fire(string playerId, int bulletInt)
    {

        BulletType bullet = (BulletType)bulletInt;

        //===================== 마스터만 실행 ======================== //
        if (PhotonNetwork.IsMasterClient)
        {
            #region 타겟 찾아서 hp 감소 처리

            string targetNick = GetNextTargetId(playerId); 
            if (string.IsNullOrEmpty(targetNick))
            {
                Debug.LogError("타겟 ID를 찾을 수 없습니다.");
                return;
            }
            //string targetId = (playerId == "player1") ? "player2" : "player1";
            Debug.Log($"{playerId}이(가) {bullet} 탄을 발사했습니다.");
            GamePlayer shooter = PlayerManager.Instance.FindPlayerByNickname(playerId);
            if (shooter == null)
            {
                Debug.LogError($"[FireSync] {playerId}에 해당하는 GamePlayer를 찾아오지 못했습니다.");
                return;
            }
            
            //shooter 애니메이션 실행 (RPC로 모든 클라이언트에서 실행되도록 함) - 마스터만 호출하기 때문에 rpc 중복 호출 x
            shooter._pv.RPC("RPC_PlayTrigger", RpcTarget.All, "Shot");
            
            
            
            //총 발사 효과음
            photonView.RPC(nameof(RPC_PlayShotSFX), RpcTarget.All, bullet==BulletType.live);
            
            if (bullet == BulletType.live)
            {
                Debug.Log($"{targetNick}이 데미지를 입었습니다.");

                // 닉네임 기반으로 먼저 찾고 → PlayerId로 변환
                GamePlayer targetByNick = PlayerManager.Instance.FindPlayerByNickname(targetNick);
                if (targetByNick != null)
                {
                    string targetPlayerId = targetByNick.PlayerId;
                    // PlayerId로 Dictionary에서 정확하게 찾아서 데미지 적용
                    if (PlayerManager.Instance.GetAllPlayers().TryGetValue(targetPlayerId, out GamePlayer target))
                    {
                        int damage = GunManager.Instance.IsEnhanced ? 2 : 1;
                        // target._pv.RPC("RPC_DecreaseHp", RpcTarget.All, damage);
                        //-----쏜 사람만 decrese 호출해주면 rpc로 적용됨 ----//
                        targetByNick.DecreaseHp(damage);
                        // target._pv.RPC(" RPC_DecreasePlayerCurrentHp", RpcTarget.All, damage);
                        Debug.Log($"[FireSync] 대상 {target.Nickname}에게 데미지 {damage} 적용됨 → 남은 HP: {target.CurrentHp}");
                    }
                    else
                    {
                        Debug.LogError($"[FireSync] PlayerId({targetPlayerId}) 기준으로도 GamePlayer를 못 찾았습니다.");
                    }
                }
                else
                {
                    Debug.LogError($"[FireSync] Nickname({targetNick}) 기준으로도 GamePlayer를 찾지 못했습니다.");
                }
            }
            else
            {
                Debug.Log($"{targetNick}이 데미지를 입지 않았습니다.");
            }

            #endregion
            
            photonView.RPC("FireResult", RpcTarget.All, targetNick, (int)bullet);
            
        }
       //=================================================//
        
        // 탄 소모 및 다음 탄 장전
        GunManager.Instance.IsEnhanced = false;
        if (!GunManager.Instance.Magazine.TryDequeue(out var next))
        {
            next = default;
        }
        GunManager.Instance.SetLoadedBullet(next);
        Debug.Log($"장전된 탄: {GunManager.Instance.LoadedBullet}, 남은 탄 수: {GunManager.Instance.Magazine.Count}");

        
        //===================== 마스터만 실행 ======================== //
        // 탄을 모두 소진했을 경우 마스터만 자동 장전 후 동기화
        if (PhotonNetwork.IsMasterClient && GunManager.Instance.LoadedBullet == default && GunManager.Instance.Magazine.Count == 0)
        {
            GunManager.Instance.Reload();

            BulletType[] current = GunManager.Instance.Magazine.ToArray();
            List<int> bullets = new List<int> { (int)GunManager.Instance.LoadedBullet };
            bullets.AddRange(Array.ConvertAll(current, b => (int)b));

            photonView.RPC(nameof(ReloadSync), RpcTarget.All, bullets.ToArray(), bullets[0]);
            ItemBoxSpawnerManager.Instance.ShowAllBoxes();
        }

       
        //=================================================//
    }
    
    
    //연출 후 턴 끝내고 동기화 맞춰주기 위해 함수 분리
    //마스터인 경우에만 이걸 호출해주면 됨
    public static void RequestEndTurn()
    {
        TurnSync turnSync = FindObjectOfType<TurnSync>(); // 직접 찾아서 호출
        if (turnSync != null)
            turnSync.photonView.RPC("RequestEndTurn", RpcTarget.MasterClient);
        else
            Debug.LogError("TurnSync를 찾을 수 없습니다.");
    }
    
    

    [PunRPC]
    public void RequestFire(string shooterId)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogWarning("[RequestFire] 마스터 클라이언트가 아님 → 무시");
            return;
        }

        BulletType loaded = GunManager.Instance.LoadedBullet;
        Debug.Log($"[RequestFire] {shooterId} 요청에 따라 마스터가 발사 처리 → 탄: {loaded}");

        // 발사 처리: 마스터가 모든 클라이언트에 발사 결과 전송
        photonView.RPC("Fire", RpcTarget.All, shooterId, (int)loaded);
    }


    [PunRPC]
    private void FireResult(string targetId, int bulletInt)
    {
        BulletType bullet = (BulletType)bulletInt;
        OnPlayerHit?.Invoke(targetId, bullet);
    }
    private IEnumerator DelayedFindAndDamage(string targetId)
    {
        float waitTime = 0f;
        float timeout = 2f; // 최대 2초 대기

        while (waitTime < timeout)
        {
            GamePlayer fallback = PlayerManager.Instance.FindPlayerByNickname(targetId);
            if (fallback != null)
            {
                int damage = GunManager.Instance.IsEnhanced ? 2 : 1;
                fallback.DecreaseHp(damage);
                Debug.Log($"[FireSync] Nickname 기준 타겟({fallback.Nickname})에게 데미지 적용 완료");
                yield break;
            }

            yield return new WaitForSeconds(0.1f);
            waitTime += 0.1f;
        }

        Debug.LogError($"[FireSync] Nickname 기준으로도 {targetId} 플레이어를 끝내 찾지 못했습니다.");
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
        Debug.Log("[ReloadSync] 받은 탄 배열: " + string.Join(", ", bullets.Select(b => ((BulletType)b).ToString())));


        StartCoroutine(Util_LDH.ShowBulletCamereaEffect());
    }

    private string GetNextTargetId(string shooterId)
    {
        var field = typeof(InGameManager).GetField("_turnOrder", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var turnOrder = field?.GetValue(InGameManager.Instance) as LinkedList<string>;

        if (turnOrder == null || turnOrder.Count <= 1)
        {
            Debug.LogWarning("턴 순서 정보가 없거나 플레이어가 부족합니다.");
            return null;
        }

        var current = turnOrder.Find(shooterId);
        if (current == null)
        {
            Debug.LogWarning($"[FireSync] shooterId({shooterId})가 턴 순서에 없습니다.");
            return null;
        }
        string nextNickname = current.Next?.Value ?? turnOrder.First.Value;

        // 다음 턴 플레이어 (끝이면 순환)
        return current.Next?.Value ?? turnOrder.First.Value;

        // 닉네임 → PlayerId 변환
        var nextPlayer = PlayerManager.Instance.FindPlayerByNickname(nextNickname);
        if (nextPlayer == null)
        {
            Debug.LogError($"[FireSync] {nextNickname} 기준으로 GamePlayer를 못 찾음 → 등록 안 됐을 수 있음");
            return null;
        }

        return nextPlayer.PlayerId;  // 진짜 PlayerId 반환
    }
    
    
    

    [PunRPC]
    public void RPC_PlayShotSFX(bool isLiveBullet)
    {
        Debug.Log($"sfx : 실탄인지? {isLiveBullet}");
        if(isLiveBullet)
            Manager.Sound.PlayFire();
        else
            Manager.Sound.PlayBlank();
    }

}
