using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;
using Managers;

using Photon.Pun;

public enum BulletType
{
    blank = 1, live = 2
}

public class GunManager : Singleton<GunManager>
{
    #region >> Constants
    private const int BULLET_MIN_COUNT = 1; // 최소 총일 갯수 2개(공포탄 1개, 실탄 1개)
    private const int BULLET_MAX_COUNT = 4; // 최대 총알 갯수 8개(공포탄 4개, 실탄 4개)
    private const int BASE_DAMAGE = 1;
    #endregion

    #region >> Variables
    private Queue<BulletType> _magazine = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length);
    public Queue<BulletType> Magazine { get { return _magazine; } private set { _magazine = value; } }
    private BulletType _loadedBullet; // 현재 장전된 탄환
    public BulletType LoadedBullet { get { return _loadedBullet; } private set { _loadedBullet = value; } }
    private bool _isEnhanced;
    public bool IsEnhanced { get { return _isEnhanced; } set { _isEnhanced = value; } }
    private PhotonView _pv;
    // 다른 곳에서 RPC 호출 가능하게
    public PhotonView PV => _pv;
    #endregion

    #region  >> Unity Message Function
    private void Awake() => Init();

    private void Init()
    {
        SingletonInit();
        _pv = GetComponent<PhotonView>();
        _isEnhanced = false;
        Manager.Game.OnTurnStart += Reload;
    }
    #endregion

    #region >> Public Function
    // public void Fire(string playerId)
    // {
    //     GamePlayer target = Manager.PlayerManager.GetAllPlayers()[playerId];
    //     if (!target)
    //     {
    //         Debug.Log($"{playerId}에 해당하는 플레이어를 찾을 수 없습니다.");
    //         return;            
    //     }
    //     _pv.RPC(nameof(RPC_Fire), RpcTarget.All, target);
    // }
    public void Fire(string firedId, string shotId)
    {
        if (PhotonNetwork.IsMasterClient)
        { 
            GamePlayer firedPlayer = Manager.PlayerManager.GetAllPlayers()[firedId];
            if (!firedPlayer)
            {
                Debug.Log($"{firedId}에 해당하는 플레이어를 찾을 수 없습니다.");
                return;
            }

            GamePlayer shotPlayer = Manager.PlayerManager.GetAllPlayers()[shotId];
            if (!shotPlayer)
            {
                Debug.Log($"{shotId}에 해당하는 플레이어를 찾을 수 없습니다.");
                return;
            }

            _pv.RPC(nameof(RPC_Fire), RpcTarget.All, firedPlayer, shotPlayer);  
        }
    }

    public void Reload()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            if (_magazine.Count <= 0 && _loadedBullet == default)
            {
                // Debug.Log("Master Reload");
                // _pv.RPC(nameof(RPC_Reload), RpcTarget.All);

                // 새 탄창 생성
                Queue<int> nextMagazine = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length);
                BulletType[] bulletTypeCountPreSet = ConvertCountDictToArray(GetRandomBulletCount());
                // TO DO: 재장전 연출 실행
                foreach (BulletType bullet in ShuffleBullets(bulletTypeCountPreSet))
                {
                    Debug.Log($"장전 >> {bullet}");
                    nextMagazine.Enqueue((int)bullet);
                }
                _pv.RPC(nameof(SyncMagazine), RpcTarget.All, nextMagazine.ToArray());

                // 다음탄 장전
                // if (!_magazine.TryDequeue(out BulletType nextBullet))
                // {
                //     nextBullet = default;
                // }
                // _pv.RPC(nameof(SyncLoadedBullet), RpcTarget.All, (int)nextBullet);
                SetNextLoadedBullet();
            }
        }
    }

    /// <summary>
    /// 총을 선택했을때 호출할 함수 => 타겟 선정 UI 호출을 위함
    /// </summary>
    public void PickUp()
    {
        // targetSelectUI를 표시함
        // 개인용
    }

    public void SwitchNextBullet()
    {
        int loopCnt = 0, maxLoop = 10;
        BulletType switchBullet = _loadedBullet;
        while (switchBullet == _loadedBullet && loopCnt < maxLoop)
        {
            switchBullet = (BulletType)new System.Random().Next(0, Enum.GetValues(typeof(BulletType)).Length);
            loopCnt++;
        }
        if (loopCnt >= maxLoop)
        {
            Debug.Log($"next bullet 변경 실패.");
            return;
        }
        _loadedBullet = switchBullet;
    }

    public BulletType GetBulletTypeByIndex(int index)
    {
        BulletType[] _magazineArray = _magazine.ToArray();
        return (index == 0) ? _loadedBullet : _magazineArray[index - 1];
    }
    #endregion

    #region >> Private Function
    // [PunRPC]
    // public void RPC_Fire(GamePlayer target)
    // {
    //     // 타겟 hp 감소
    //     if (_loadedBullet == BulletType.live)
    //     {
    //         int damage = _isEnhanced ? BASE_DAMAGE * 2 : BASE_DAMAGE;
    //         target.DecreaseHp(damage);
    //         Debug.Log("target hp 감소");
    //     }

    //     // TO DO: 탄피 배출 연출 실행
    //     // _isEnhanced = false;
    //     _pv.RPC(nameof(SyncEnhancement), RpcTarget.All, false);

    //     // 장전
    //     BulletType nextBullet;
    //     if (!_magazine.TryDequeue(out nextBullet))
    //     {
    //         nextBullet = default;
    //     }

    //     // 턴 종료
    //     Manager.Game.EndTurn();
    // }
    [PunRPC]
    public void RPC_Fire(GamePlayer firedPlayer, GamePlayer shotPlayer)
    {
        // 턴이 넘어가는 기준
        // 나 -> 상대(실탄/공포탄): 턴 넘어감
        // 나 -> 나(실탄): 턴 넘어감
        // 나 -> 나(공포탄): 턴 넘어가지 않음
        bool isPassed = true;
        if (_loadedBullet == BulletType.live)
        {
            // TO DO: 실탄 발포 연출 실행
            int damage = _isEnhanced ? BASE_DAMAGE * 2 : BASE_DAMAGE;
            shotPlayer.DecreaseHp(damage);
            Debug.Log("[GunManager] target hp 감소.");
        }
        else if (_loadedBullet == BulletType.blank)
        {
            // TO DO: 공포탄 발포 연출 실행
            Debug.Log("[GunManager] target hp 감소하지 않음.");
            if (firedPlayer.PlayerId == shotPlayer.PlayerId)
            {
                isPassed = false;
            }
        }

        // TO DO: 탄피 배출 연출 실행
        _isEnhanced = false;
        _pv.RPC(nameof(SyncEnhancement), RpcTarget.All, false);

        // 다음탄 장전
        // if (!_magazine.TryDequeue(out BulletType nextBullet))
        // {
        //     nextBullet = default;
        // }
        // _pv.RPC(nameof(SyncLoadedBullet), RpcTarget.All, nextBullet);
        SetNextLoadedBullet();

        // 턴 종료
        Manager.Game.EndTurn(isPassed);
    }
    [PunRPC]
    private void RPC_Reload()
    {
        // 새 탄창 생성
        Queue<int> nextMagazine = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length);
        BulletType[] bulletTypeCountPreSet = ConvertCountDictToArray(GetRandomBulletCount());
        // TO DO: 재장전 연출 실행
        foreach (BulletType bullet in ShuffleBullets(bulletTypeCountPreSet))
        {
            Debug.Log($"장전 >> {bullet}");
            nextMagazine.Enqueue((int)bullet);
        }
        _pv.RPC(nameof(SyncMagazine), RpcTarget.All, nextMagazine.ToArray());

        // 다음탄 장전
        // if (!_magazine.TryDequeue(out BulletType nextBullet))
        // {
        //     nextBullet = default;
        // }
        // _pv.RPC(nameof(SyncLoadedBullet), RpcTarget.All, (int)nextBullet);
        SetNextLoadedBullet();
    }

    private BulletType[] ShuffleBullets(BulletType[] bulletSet)
    {
        for (int cnt = 0; cnt < bulletSet.Length; cnt++)
        {
            int changeIndex = new System.Random().Next(0, cnt + 1);
            (bulletSet[changeIndex], bulletSet[cnt]) = (bulletSet[cnt], bulletSet[changeIndex]);
        }
        return bulletSet;
    }

    private Dictionary<BulletType, int> GetRandomBulletCount()
    {
        Dictionary<BulletType, int> result = new();
        foreach (BulletType bulletType in Enum.GetValues(typeof(BulletType)))
        {
            result.Add(bulletType, new System.Random().Next(BULLET_MIN_COUNT, BULLET_MAX_COUNT + 1));
        }
        return result;
    }

    private BulletType[] ConvertCountDictToArray(Dictionary<BulletType, int> bulletTypeCountSet)
    {
        List<BulletType> preSet = new();
        foreach (KeyValuePair<BulletType, int> item in bulletTypeCountSet)
        {
            for (int cnt = 0; cnt < item.Value; cnt++)
            {
                preSet.Add(item.Key);
            }
        }
        return preSet.ToArray();
    }

    private Queue<BulletType> ConvertIntToBulletTypeQueue(int[] intBulletTypeArray)
    {
        Queue<BulletType> result = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length);
        for (int cnt = 0; cnt < intBulletTypeArray.Length; cnt++)
        {
            result.Enqueue((BulletType)intBulletTypeArray[cnt]);
        }
        return result;
    }

    private void SetNextLoadedBullet()
    { 
        // 다음탄 장전
        if (!_magazine.TryDequeue(out BulletType nextBullet))
        {
            nextBullet = default;
        }
        _pv.RPC(nameof(SyncLoadedBullet), RpcTarget.All, (int)nextBullet);
    }
    #endregion

    /// <summary>
    /// 현재 장전은 마스터 클라이언트만 장전하고 탄창을 클라이언트와 공유하는형식
    /// 마스터는 Reload를 호출하여_loadedBullet 값을 변경하지만
    /// 클라이언트는 Reload함수를 실행시키지않음으로 값을 변경하지않으면 읽지 못함
    /// 클라이언트도 값이 변경되어 확인할 수 있는 set함수 
    /// </summary>
    public void SetLoadedBullet(BulletType bullet)
    {
        _loadedBullet = bullet;
    }

    [PunRPC]
    public void RPC_SwitchNextBullet()
    {
        int loopCnt = 0, maxLoop = 10;
        BulletType switchBullet = _loadedBullet;
        while (switchBullet == _loadedBullet && loopCnt < maxLoop)
        {
            switchBullet = (BulletType)new System.Random().Next(0, Enum.GetValues(typeof(BulletType)).Length);
            loopCnt++;
        }

        if (loopCnt >= maxLoop)
        {
            Debug.Log("다이얼 사용 실패 → 다른 탄환으로 교체 불가");
            return;
        }

        _loadedBullet = switchBullet;
        Debug.Log($"다이얼 사용 성공 → 현재 탄환: {_loadedBullet}");
    }

    #region Sync
    [PunRPC]
    private void SyncMagazine(int[] magazine)
    {
        _magazine = ConvertIntToBulletTypeQueue(magazine);
        foreach (BulletType type in _magazine)
        {
            Debug.Log($"{type}");
        }
    }
    [PunRPC]
    private void SyncLoadedBullet(int loadedBullet)
    {
        _loadedBullet = (BulletType)loadedBullet;
    }
    [PunRPC]
    private void SyncEnhancement(bool isEnhanced)
    {
        _isEnhanced = isEnhanced;
    }
    #endregion
}
