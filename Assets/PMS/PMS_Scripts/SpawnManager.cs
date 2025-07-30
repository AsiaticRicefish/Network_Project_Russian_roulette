using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using ExitGames.Client.Photon;

public class SpawnManager : MonoBehaviourPunCallbacks
{
    public static SpawnManager Instance;

    //룸에서 사용되는 룸 프로퍼티 접두사
    public const string SP_KEY_PREFIX = "SpawnPoint_";      //S

    private Transform[] _allSpawnPoints;


    //여기까지는 모든 유저가 해도됨.
    private void Awake()
    {
        //방마다 존재 + 초반 게임 시작후 자리 배정할 때만 사용
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;


        // 만약 Inspector에서 설정하지 않았다면, 여기에서 자식 Transform들을 스폰 포인트로 초기화
        if (_allSpawnPoints == null || _allSpawnPoints.Length == 0)
        {
            // SpawnManager 자식 객체들을 스폰 포인트로 사용
            List<Transform> children = new List<Transform>();
            //해당 foreach문은 직계 자식들의 transform만 순회
            foreach (Transform child in transform)
            {
                children.Add(child);
            }
            _allSpawnPoints = children.ToArray();
            Debug.Log($"SpawnManager : {_allSpawnPoints.Length}개의 스폰포인트를 찾았습니다.");
        }

        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log("스폰 초기화");
            InitializeAvailableSpawnPoints(); //마스터클라이언트만 룸 프로퍼티 스폰 초기화 
        }
        else
        {
            StartCoroutine(InitDelay());
        }
    }

    //이 함수의 예상 호출 시점은 게임시작 할 때
    public void InitializeAvailableSpawnPoints()
    {
        if (!PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 아니면 실행하지 않음
        {
            Debug.LogWarning("마스터 클라이언트만 초기화 할 수 있음");
            return;
        }

        //Photon 전용 HashTable 사용
        ExitGames.Client.Photon.Hashtable customProperties = new ExitGames.Client.Photon.Hashtable();

        for (int i = 0; i < _allSpawnPoints.Length; i++)
        {
            // 각 스폰 지점의 상태를 "사용 가능" (true)으로 설정하여 Hashtable에 추가       
            customProperties.Add(SP_KEY_PREFIX + i.ToString(), true);
            Debug.Log(SP_KEY_PREFIX + i);
        }
        // 설정된 프로퍼티를 현재 방에 적용 -> 모든 클라이언트에 동기화
        PhotonNetwork.CurrentRoom.SetCustomProperties(customProperties);

        Debug.Log($"SpawnManager: Initialized {customProperties.Count} spawn point properties in room.");

        /*foreach(var c in customProperties)
        {
            Debug.Log($"key : {c.Key}, value : {c.Value} ,");
        }*/
    }

    /// <summary>
    /// 마스터 클라이언트에서만 호출, 사용 가능한 스폰 지점 중 하나를 선택하고,
    /// 룸 프로퍼티에서 해당 지점을 사용중 false로 설정
    /// 여기 Tuple로 반환 받는 이유는 나중에 index값을 통해 어떤 자리를 들고왔는지 알고 이후 자리를 반납할 때 필요하기 때문에 
    /// </summary>
    /// <returns>선택된 스폰 지점 Transform과 _allSpawnPoints 배열에서의 인덱스. 없으면 (null, -1)</returns>
    public (Transform spawnPoint, int index) GetAndClaimRandomSpawnPoint()
    {
        /*if (!PhotonNetwork.IsMasterClient) // 마스터 클라이언트가 아니면 오류 로그 출력
        {
            Debug.LogError("마스터 클라이언트만 룸 프로퍼티에 대한 부분을 설정할 수 있음.");
            return (null, -1);
        }*/
        List<int> availableIndices = new List<int>();

        // 모든 스폰 지점을 순회하며 룸 프로퍼티에서 사용 가능 상태(값이 true)인 스폰 지점의 인덱스를 수집
        for (int i = 0; i < _allSpawnPoints.Length; i++)
        {
            string key = SP_KEY_PREFIX + i.ToString();
            Debug.Log($"키이름{key}");
            // 룸 프로퍼티에 해당 키가 존재하고, 값이 true(사용 가능)인지 확인
            //(bool)PhotonNetwork.CurrentRoom.CustomProperties[key] 커스텀 프로퍼티에 등록되어 있을 때 object타입으로 들어가 있음
            //하지만 실제 우리는 bool타입이 들어가 있는거 알 기 때문에 명시적 캐스팅이 가능하다.
            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key))
            {
                Debug.Log("들어있음");
            }
            else
            {
                Debug.Log("안들어 있음");
            }

            if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) &&
                (bool)PhotonNetwork.CurrentRoom.CustomProperties[key] == true)
            {
                Debug.Log($"반복 호출 횟수 : {i}");
                availableIndices.Add(i); // 사용 가능한 스폰 지점 인덱스 리스트에 추가
            }
        }

        if (availableIndices.Count == 0) // 사용 가능한 스폰 지점이 없으면
        {
            Debug.LogError("모든 스폰 지점이 사용 중입니다. 더 이상 스폰할 수 없습니다.");
            return (null, -1);
        }

        //제일 빠른 위치에 앉게하기
        int selectedIndex = availableIndices[0];
        Transform selectedSpawnPoint = _allSpawnPoints[selectedIndex];

        // 룸 프로퍼티 업데이트: 선택된 스폰 지점을 사용 중(false)으로 설정
        ExitGames.Client.Photon.Hashtable propsToSet = new ExitGames.Client.Photon.Hashtable();
        propsToSet.Add(SP_KEY_PREFIX + selectedIndex.ToString(), false);

        // SetCustomProperties를 호출하여 변경된 프로퍼티를 모든 클라이언트에 동기화
        PhotonNetwork.CurrentRoom.SetCustomProperties(propsToSet);

        return (selectedSpawnPoint, selectedIndex);
    }

    /// <summary>
    /// 마스터 클라이언트만 써야하기는 하는데 유저가 나가면 해당 함수가 호출 되게 해야함. 
    /// 사용이 끝난 스폰 지점을 다시 사용 가능(true) 상태로 되돌리는 함수
    /// </summary>
    /// <param name="spawnPointIndex">반환할 스폰 지점의 _allSpawnPoints 인덱스</param>
    public void ReturnSpawnPoint(int spawnPointIndex)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            Debug.LogError("마스터 클라이언트만 자리를 되돌려 놓을 수 있다");
            return;
        }

        //자리 index 유효값 검사
        if (spawnPointIndex < 0 || spawnPointIndex >= _allSpawnPoints.Length)
        {
            Debug.LogError($"해당 인덱스 {spawnPointIndex}번은 범위를 벗어났습니다");
            return;
        }

        string key = SP_KEY_PREFIX + spawnPointIndex.ToString();

        //해당 인덱스를 통한 자리가 이미 사용 가능 자리이면 return
        if (PhotonNetwork.CurrentRoom.CustomProperties.ContainsKey(key) &&
            (bool)PhotonNetwork.CurrentRoom.CustomProperties[key] == true)
        {
            return;
        }

        // 룸 프로퍼티 업데이트: 스폰 지점을 사용 가능(true) 으로 설정
        ExitGames.Client.Photon.Hashtable props = new ExitGames.Client.Photon.Hashtable();
        props.Add(key, true);
        PhotonNetwork.CurrentRoom.SetCustomProperties(props);
        Debug.Log($"SpawnManager: Master Client returned spawn point index {spawnPointIndex}.");
    }

    private IEnumerator InitDelay()
    {
        yield return new WaitForSeconds(0.1f);
    }
}
