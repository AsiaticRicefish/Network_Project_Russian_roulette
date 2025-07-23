using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button leaveButton;
    [SerializeField] private GameObject playerPanelPrefabs;
    [SerializeField] private Transform hostPanel;
    [SerializeField] private Transform clientPanel;

    private GameObject hostInstance;
    private GameObject clientInstance;

    private void Start()
    {
        startButton.onClick.AddListener(GameStart);
        leaveButton.onClick.AddListener(LeaveRoom);
    }

    // 개별 플레이어 패널 생성
    public void PlayerPanelSpawn(Player player)
    {
        Transform targetParent = player.IsMasterClient ? hostPanel : clientPanel;

        GameObject obj = Instantiate(playerPanelPrefabs);
        obj.transform.SetParent(targetParent, false);
        PlayerPanel item = obj.GetComponent<PlayerPanel>();
        item.Init(player);

        if (player.IsMasterClient)
            hostInstance = obj;
        else
            clientInstance = obj;
    }

    // 방장 및 모든 플레이어가 준비완료시 시작
    public void GameStart()
    {
        if (PhotonNetwork.IsMasterClient && AllPlayerReadyCheck())
            PhotonNetwork.LoadLevel("TestGameScene");
    }

    // 모든플레이어가 준비완료 상태인지
    public bool AllPlayerReadyCheck()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            if (!player.CustomProperties.TryGetValue("Ready", out object value) || !(bool)value)
                return false;
        }
        return true;
    }

    // 플레이어가 나갈때 패널 제거
    public void PlayerPanelDestroy(Player player)
    {
        // 나간 사람이 호스트인 경우
        if (player.IsMasterClient && hostInstance != null)
        {
            Destroy(hostInstance);
            hostInstance = null;
        }
        // 나간 사람이 클라이언트인 경우
        else if (!player.IsMasterClient && clientInstance != null)
        {
            Destroy(clientInstance);
            clientInstance = null;
        }
    }
    public void LeaveRoom()
    {
        if (hostInstance == null)
            return;

        Destroy(hostInstance);
        hostInstance = null;

        if (clientInstance == null)
            return;

        Destroy(clientInstance);
        clientInstance = null;

        PhotonNetwork.LeaveRoom(); // 방 나가기
    }
}
