using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerPanel : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI nicknameText;
    [SerializeField] private TextMeshProUGUI readyText;
    [SerializeField] private Image readyButtonImage;
    [SerializeField] private Button readyButton;

    private bool isReady;

    // 플레이어 UI 초기화
    public void Init(Player player)
    {
        nicknameText.text = player.NickName;
        if (player.IsMasterClient)
            nicknameText.color = Color.yellow;
        else
            nicknameText.color = Color.black;
        readyButton.interactable = player.IsLocal;

        if (!player.IsLocal)
            return;

        isReady = false;
        ReadyPropertyUpdate();

        readyButton.onClick.RemoveListener(ReadyButtonClick);
        readyButton.onClick.AddListener(ReadyButtonClick);
    }

    // 준비버튼 클릭 함수
    public void ReadyButtonClick()
    {
        isReady = !isReady;

        readyText.text = isReady ? "Ready" : "Click Ready";
        readyButtonImage.color = isReady ? Color.yellow : Color.grey;
        ReadyPropertyUpdate();
    }
    //준비상태 저장
    public void ReadyPropertyUpdate()
    {
        ExitGames.Client.Photon.Hashtable playerProperty = new Hashtable();
        playerProperty["Ready"] = isReady;
        PhotonNetwork.LocalPlayer.SetCustomProperties(playerProperty);
    }

    // 다른 플레이어 준비 상태 체크
    public void ReadyCheck(Player player)
    {
        if (player.CustomProperties.TryGetValue("Ready", out object value))
        {
            readyText.text = (bool)value ? "Ready" : "Click Ready";
            readyButtonImage.color = (bool)value ? Color.yellow : Color.grey;
        }
    }
}
