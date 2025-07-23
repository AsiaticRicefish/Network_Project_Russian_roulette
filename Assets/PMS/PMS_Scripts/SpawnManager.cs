using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//방마다 존재 + 초반 게임 시작후 자리 배정할 때만 사용
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;

    [SerializeField] private Transform[] spawnPoints;

    private void Awake()
    {
        Instance = this;
    }

    public Transform GetSpawnPoint(int index)
    {
        if (index >= 0 && index < spawnPoints.Length)
            return spawnPoints[index];

        return spawnPoints[0];
    }
}
