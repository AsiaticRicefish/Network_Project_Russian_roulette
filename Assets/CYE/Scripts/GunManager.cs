using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;
using System;


public enum BulletType
{
    blank, live
}

public class GunManager : Singleton<GunManager>
{
    private const int BULLET_MIN_COUNT = 1; // 최소 총일 갯수 2개(공포탄 1개, 실탄 1개)
    private const int BULLET_MAX_COUNT = 4; // 최대 총알 갯수 8개(공포탄 4개, 실탄 4개)
    
    private Queue<BulletType> _magazine = new(BULLET_MAX_COUNT * Enum.GetValues(typeof(BulletType)).Length); // capacity 지정
    public Queue<BulletType> Magazine { get { return _magazine; } private set { _magazine = value; } }
    private BulletType _loadedBullet; // 현재 장전된 탄환

    private void Awake() => SingletonInit();

    public void Fire() // Fire(Player target)
    {
        if (_loadedBullet == BulletType.live)
        {
            // target에게 대미지를 부여함
        }
        // TO DO: 탄피 배출 연출 실행
        if (_magazine.TryDequeue(out _loadedBullet))
        {
            // 다음 탄 장전 성공
        }
        else
        {
            // 다음 탄 장전 실패
            Reload();
        }
    }
    public void Reload()
    {
        // TO DO: 재장전 연출 실행
    }
    private void GetBulletSet()
    { 
        // 탄창의 랜덤한 배열을 생성하고 _magazine에 값을 지정함
    }
    private void GetBulletCount()
    {
        // 각 BulletType의 랜덤한 갯수를 가져옴 => Next(BULLET_MIN_COUNT, BULLET_MAX_COUNT + 1)
    }
}
