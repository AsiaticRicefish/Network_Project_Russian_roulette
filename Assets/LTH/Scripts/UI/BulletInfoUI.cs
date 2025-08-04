using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class BulletInfoUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI bulletInfoText;

    private float updateInterval = 0.2f;
    private float timer;
    private string myId;

    private void Start()
    {
        myId = PhotonNetwork.NickName;
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < updateInterval) return;
        timer = 0f;

        string turnInfo = Managers.Manager.Game.CurrentTurn == myId
            ? "<color=green> 내 턴입니다</color>"
            : "<color=red> 내 턴이 아닙니다</color>";

        bulletInfoText.text = turnInfo;
    }
}