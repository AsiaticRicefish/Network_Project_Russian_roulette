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
    private PhotonView _pv;

    private void Awake()
    {
        SingletonInit();
        _pv = gameObject.AddComponent<PhotonView>(); // 동적으로 PhotonView 추가

        if (_pv.ViewID == 0)
        {
            bool success = PhotonNetwork.AllocateViewID(_pv); // ViewID 수동 할당
            Debug.Log($"[GunManager] ViewID 할당됨: {_pv.ViewID}, 성공여부: {success}");
        }

        _isEnhanced = false;

        Manager.Game.OnTurnStart += () =>
        {
            GunManager.Instance.PV.RPC("RPC_SetEnhanced", RpcTarget.All, false);
            Reload();
        };

    }



    // 다른 곳에서 RPC 호출 가능하게
    public PhotonView PV => _pv;

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
    #endregion

    #region  >> Unity Message Function
    //private void Awake() => Init();

    private void Init()
    {
        SingletonInit();
        _isEnhanced = false;
        Manager.Game.OnTurnStart += Reload;
    }
    #endregion

    #region >> Public Function
    // public void Fire(GamePlayer target) => FireGunToTarget(target);
    // {
    //     if (_loadedBullet == BulletType.live)
    //     {
    //         int damage = _isEnhanced ? BASE_DAMAGE * 2 : BASE_DAMAGE;
    //         target.DecreaseHp(damage);
    //     }
    //     // TO DO: 탄피 배출 연출 실행
    //     _isEnhanced = false;
    //     _magazine.TryDequeue(out _loadedBullet);
    // }
    public void Fire(string playerId)
    {
        GamePlayer target = Manager.PlayerManager.GetAllPlayers()[playerId];
        if (!target)
        {
            Debug.Log($"{playerId}에 해당하는 플레이어를 찾을 수 없습니다.");
            return;
        }
        FireGunToTarget(target);
    }

    public void Reload()
    {
        // TO DO: 해당 함수 사용처에서 충분히 테스트가 진행되었다면 다시 하단의 조건을 주석 해제하여야한다.
        if (_magazine.Count <= 0 && _loadedBullet == default)
        {
            BulletType[] bulletTypeCountPreSet = ConvertCountDictToArray(GetRandomBulletCount());
            // TO DO: 재장전 연출 실행(반드시 ShuffleBullets 호출 전에 실행하여야 한다.)
            foreach (BulletType bullet in ShuffleBullets(bulletTypeCountPreSet))
            {
                Debug.Log($"장전 >> {bullet}");
                _magazine.Enqueue(bullet);
            }
            if (!_magazine.TryDequeue(out _loadedBullet))
            {
                Debug.Log("장전 실패.");
                _loadedBullet = default;
                return;
            }
        }
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
    private void FireGunToTarget(GamePlayer target)
    {
        if (_loadedBullet == BulletType.live)
        {
            int damage = _isEnhanced ? BASE_DAMAGE * 2 : BASE_DAMAGE;
            target.DecreaseHp(damage);
        }
        // TO DO: 탄피 배출 연출 실행
        _isEnhanced = false;
        if (!_magazine.TryDequeue(out _loadedBullet))
        {
            _loadedBullet = default;
        }
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

    [PunRPC]
    public void RPC_SetEnhanced(bool value)
    {
        _isEnhanced = value;
        Debug.Log($"[동기화] isEnhanced = {value}");
    }


    [PunRPC]
    public void RPC_PlayShotSFX(bool isLiveBullet)
    {
        if(isLiveBullet)
            Manager.Sound.PlayFire();
        else
            Manager.Sound.PlayBlank();
    }

}