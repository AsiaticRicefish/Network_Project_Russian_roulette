using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Managers;
using System;
using ExitGames.Client.Photon.StructWrapping;
using GameCamera;
using Photon.Realtime;
using Utils;
using Hashtable = ExitGames.Client.Photon.Hashtable;


public class SceneInit : MonoBehaviourPunCallbacks
{
    [SerializeField] private string playerPrefabName;

    [SerializeField] private Transform[] spawnPoints;
    [SerializeField] private CinemachineVirtualCamera[] virtualCameras;


    private void Awake()
    {
        SceneManager.sceneLoaded += OnEnterScene;
    }


    private void Start()
    {
        //if (SceneManager.GetActiveScene().name != "LTH_GameScene") return;
        //
        // if (PhotonNetwork.IsConnectedAndReady && PhotonNetwork.InRoom)
        // {
        //     StartCoroutine(InitFlow());
        // }


        Manager.PlayerManager.OnAddPlayer += InitGame;
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnEnterScene;
        //Manager.PlayerManager.OnAddPlayer -= InitGame;
    }


    //둘 다 씬에 들어왔을 때 실행해줘야 하는데
    private IEnumerator InitFlow()
    {
        Debug.Log("[SceneInit] 모든 플레이어가 씬에 들어와 InitFlow() 실행");
        // 플레이어 스폰
        yield return SpawnPlayerWithDelay();
    }

    private void InitGame()
    {
        if (Manager.PlayerManager.GetAllPlayers().Count == 2)
        {
            Manager.Camera.PushCamera("Player");
            if (PhotonNetwork.IsMasterClient)
                InGameManager.Instance.StartGame();
        }

        // // 마스터만 게임 시작 진행
        // if (PhotonNetwork.IsMasterClient)
        // {
        //     yield return WaitForBoxSpawner();
        //
        //     // 게임 시작
        //     
        // }
    }


    private IEnumerator SpawnPlayerWithDelay()
    {
        while (!PhotonNetwork.IsConnectedAndReady)
            yield return null;

        yield return new WaitUntil(() => GameObject.Find("GameTable") != null);

        Transform tableCenter = GameObject.Find("GameTable").transform;

        //int actorIndex = PhotonNetwork.LocalPlayer.ActorNumber - 1; // actornumber로 하면 안됨 actor number는 방에 들어오는 플레이어마다 누적시켜서 카운팅되므로 나갔다 들어오면 증기함
        int actorIndex = PhotonNetwork.IsMasterClient ? 0 : 1;
        Vector3 spawnPos = spawnPoints.Length > actorIndex ? spawnPoints[actorIndex].position : Vector3.zero;


        //bulletdisaplay virtual cam 할당 및 설정
        Util_LDH.GetOrAddComponent<VirtualCam_BulletDisplay>(virtualCameras[actorIndex].gameObject);


        var obj = PhotonNetwork.Instantiate(playerPrefabName, spawnPos, Quaternion.identity);

        Vector3 dir = (tableCenter.position - obj.transform.position).normalized;
        dir.y = 0f;
        obj.transform.rotation = Quaternion.LookRotation(dir);

        GamePlayer gp = obj.GetComponent<GamePlayer>();
        if (gp != null && gp.GetComponent<PhotonView>().IsMine)
        {
            string nickname = PhotonNetwork.NickName;
            string playerId = PhotonNetwork.LocalPlayer.UserId;

            gp._data = new PlayerData(nickname, playerId, 0, 0);
            gp.SendMyPlayerDataRPC();

            // 카메라 회전 보정
            Camera cam = obj.GetComponentInChildren<Camera>();
            if (cam != null)
            {
                cam.transform.rotation =
                    Quaternion.LookRotation((tableCenter.position - cam.transform.position).normalized);
            }
        }
    }

    private IEnumerator WaitForBoxSpawner()
    {
        float timer = 0f;
        float timeout = 2f;

        while (!ItemBoxSpawnerManager.Instance?.IsInitialized ?? true)
        {
            if (timer > timeout)
            {
                Debug.LogWarning("[SceneInit] 아이템 박스 스포너 초기화 대기 타임아웃!");
                break;
            }

            yield return new WaitForSeconds(0.1f);
            timer += 0.1f;
        }
    }


    private void OnEnterScene(Scene scene, LoadSceneMode mode)
    {
        //자기 자신의 isInGameScene 프로퍼티 초기화
        Hashtable playerProperty = new Hashtable { { "IsInGameScene", true } };

        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    public override void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        base.OnPlayerPropertiesUpdate(targetPlayer, changedProps);
        foreach (var player in PhotonNetwork.PlayerList)
        {
            if (player.CustomProperties.TryGetValue("IsInGameScene", out bool value))
            {
                if (!value)
                {
                    Debug.Log("[SceneInit] 아직 모든 플레이어가 ingame scene으로 들어오지 않았습니다.");

                    return;
                }
            }
        }

        StartCoroutine(InitFlow());
    }
}