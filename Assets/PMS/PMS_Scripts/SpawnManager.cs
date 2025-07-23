using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//�渶�� ���� + �ʹ� ���� ������ �ڸ� ������ ���� ���
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
