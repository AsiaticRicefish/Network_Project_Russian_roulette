using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DesignPattern;


public enum BulletType
{
    blank, live
}

public class GunManager : Singleton<GunManager>
{
    private const int BULLET_MIN_COUNT = 1; // 최소 총일 갯수 2개(공포탄 1개, 실탄 1개)
    private const int BULLET_MAX_COUNT = 4; // 최대 총알 갯수 8개(공포탄 4개, 실탄 4개)
    private Queue<BulletType> _magazine;

    private void Awake() => SingletonInit();

    private void Fire()
    {
        // if(_magazine.TryDequeue(out BulletType nextBullet))
        //  if(nextBullet)
    }
    private void Reload()
    {
        // 
        // 재장전 연출 실행

    }
}
