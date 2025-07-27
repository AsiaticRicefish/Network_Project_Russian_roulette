using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;
using Managers;


public enum BulletType
{
    blank, live
}

public class GunManager : Singleton<GunManager>
{
    private const int BULLET_MIN_COUNT = 1; // 최소 총일 갯수 2개(공포탄 1개, 실탄 1개)
    private const int BULLET_MAX_COUNT = 4; // 최대 총알 갯수 8개(공포탄 4개, 실탄 4개)
    private const int BASE_DAMAGE = 1;

    private Queue<BulletType> _magazine = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length); // capacity 지정
    public Queue<BulletType> Magazine { get { return _magazine; } private set { _magazine = value; } }
    private BulletType _loadedBullet; // 현재 장전된 탄환
    private bool _isEnhanced;

    private void Awake() => Init();

    private void Init()
    {
        SingletonInit();
        _isEnhanced = false;
        Manager.Game.OnTurnStart += Reload;
    }
    public void Fire(GamePlayer target)
    {
        if (_loadedBullet == BulletType.live)
        {
            int damage = _isEnhanced ? BASE_DAMAGE * 2 : BASE_DAMAGE;
            target.DecreaseHp(damage);
        }
        // TO DO: 탄피 배출 연출 실행
        _isEnhanced = false;
        _magazine.TryDequeue(out _loadedBullet);
    }
    public void Reload()
    {
        // if (_magazine.Count <= 0)
        {
            Dictionary<BulletType, int> bulletTypeCountSet = GetRandomBulletCount();
            // TO DO: 재장전 연출 실행
            foreach (BulletType bullet in ShuffleBullets(bulletTypeCountSet))
            {
                Debug.Log($"{bullet}");
                _magazine.Enqueue(bullet);
            }
        }
    }
    private BulletType[] ShuffleBullets(Dictionary<BulletType, int> bulletTypeCountSet)
    {
        List<BulletType> preSet = new();
        foreach (KeyValuePair<BulletType, int> item in bulletTypeCountSet)
        {
            for (int cnt = 0; cnt < item.Value; cnt++)
            {
                preSet.Add(item.Key);
            }
        }
        BulletType[] bulletSet = preSet.ToArray();
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
    public void SwitchNextBullet()
    {
        // 현재 동작 안함
        int loopCnt = 0, maxLoop = 10;
        BulletType switchBullet = _loadedBullet;
        BulletType[] types = (BulletType[])Enum.GetValues(typeof(BulletType));
        while (switchBullet != _loadedBullet || loopCnt >= maxLoop)
        {
            switchBullet = (BulletType)new System.Random().Next(0, types.Length);
            loopCnt++;
        }
        if (loopCnt >= maxLoop)
        {
            Debug.Log($"next bullet 변경 실패.");
            return;
        }
        else
        {
            Debug.Log($"{_loadedBullet}");
            _loadedBullet = switchBullet;
        }
    }
}
