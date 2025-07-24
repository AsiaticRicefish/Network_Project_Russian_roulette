using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance;
    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private List<Transform> _availableSpawnPoints; // 현재 사용 가능한 스폰 지점 리스트

    private void Awake()
    {
        //방마다 존재 + 초반 게임 시작후 자리 배정할 때만 사용
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); 
            return;
        }
        Instance = this;

        // 만약 Inspector에서 설정하지 않았다면, 여기에서 자식 Transform들을 스폰 포인트로 초기화
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            // SpawnManager 자식 객체들을 스폰 포인트로 사용
            List<Transform> children = new List<Transform>();
            //해당 foreach문은 직계 자식들의 transform만 순회
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            spawnPoints = children.ToArray();
            Debug.Log($"SpawnManager : {spawnPoints.Length}개의 스폰포인트를 찾았습니다.");
        }
    }

    private void Start()
    {
        InitializeAvailableSpawnPoints();
    }

    // 게임 시작 시 또는 라운드 시작 시 호출하여 사용 가능한 스폰 포인트를 초기화
    public void InitializeAvailableSpawnPoints()
    {
        _availableSpawnPoints = new List<Transform>(spawnPoints); // 모든 스폰 지점을 사용 가능으로 초기화
        Debug.Log($"스폰 매니저 초기화! 생성된 사용가능한 SpawnPoints의 개수: {_availableSpawnPoints.Count}");
    }

    /// <summary>
    /// 사용 가능한 스폰 지점 중 하나를 반환, 해당 지점 사용하지 못하도록 사용가능한 스폰 포인트 List에서 제거
    /// </summary>
    /// <returns>사용 가능한 스폰 지점 Transform, 없으면 null</returns>
    public Transform GetRandomAvailableSpawnPoint()
    {
        if (_availableSpawnPoints.Count == 0)
        {
            Debug.LogError("모든 스폰 지점이 사용 중입니다. 더 이상 스폰할 수 없습니다.");
            return null;
        }

        int randomIndex = Random.Range(0, _availableSpawnPoints.Count);
        Transform selectedSpawnPoint = _availableSpawnPoints[randomIndex];
        _availableSpawnPoints.RemoveAt(randomIndex); // 선택된 스폰 지점은 사용 불가로 표시

        Debug.Log($"SpawnManager: Assigned spawn point {selectedSpawnPoint.name}. Remaining available: {_availableSpawnPoints.Count}");
        return selectedSpawnPoint;
    }

    /// <summary>
    /// 사용이 끝난 스폰 지점을 다시 사용 가능하게 리스트에 다시 추가하는 메서드 - 여기까지 필요할까 싶긴한데?
    /// (예: 플레이어가 사망하거나 방을 나갈 때)
    /// </summary>
    /// <param name="spawnPointToReturn"> 반환할 스폰 지점 Transform</param>
    public void ReturnSpawnPoint(Transform spawnPointToReturn)
    {
        if (spawnPointToReturn == null) return;

        // _allSpawnPoints에 실제로 존재하는 스폰 포인트인지 확인하는 로직 추가 가능
        if (!_availableSpawnPoints.Contains(spawnPointToReturn))
        {
            _availableSpawnPoints.Add(spawnPointToReturn);
            Debug.Log($"SpawnManager: Returned spawn point {spawnPointToReturn.name}. Available count: {_availableSpawnPoints.Count}");
        }
        else
        {
            Debug.LogWarning($"SpawnManager: Spawn point {spawnPointToReturn.name} was already available.");
        }
    }

}
