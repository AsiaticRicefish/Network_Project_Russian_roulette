using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class UI_GunController : MonoBehaviour
{
    [SerializeField] private Button fireButton;
    [SerializeField] private FireSync fireSync;
    [SerializeField] private TextMeshProUGUI hitMessageText;

    /// <summary>
    /// PhotonNetwork.NickName
    /// </summary>
    private string myId;

    private bool isFiring = false; // 발사 중인지 여부

    private void Start()
    {
        myId = PhotonNetwork.NickName;

        fireButton.onClick.AddListener(OnFireButtonClicked);

        // 초기 메시지 비우기
        if (hitMessageText != null)
            hitMessageText.text = "";

        Debug.Log("[UI_GunController] 이벤트 구독 시도");
        // 이벤트 구독
        FireSync.OnPlayerHit += HandlePlayerHit;
    }

    private void OnDestroy()
    {
        // 이벤트 구독 해제
        FireSync.OnPlayerHit -= HandlePlayerHit;
    }

    private void Update()
    {
        // 발사 중이 아닐 때만 버튼 활성화
        fireButton.interactable = !isFiring && TurnSync.CurrentTurnPlayerId == myId;
    }

    private void OnFireButtonClicked()
    {
        if (isFiring)
        {
            Debug.LogWarning("[FireButtonController] 이미 발사 처리 중 → 무시");
            return;
        }

        if (TurnSync.CurrentTurnPlayerId != myId)
        {
            Debug.LogWarning("[FireButtonController] 내 턴이 아님 → 발사 안 됨");
            return;
        }

        isFiring = true;

        //중복 클릭 방지를 위해 누르자마자 interactable 차단
        fireButton.interactable = false;
        
        fireSync.photonView.RPC("RequestFire", RpcTarget.MasterClient, myId, fireSync.GetNextTargetId(myId));
    }

    // 맞은 사람과 탄 종류에 따라 메시지 출력
    private void HandlePlayerHit(string targetId, BulletType bullet)
    {
        //target id 파싱
        targetId = Util_LDH.GetUserNickname(targetId);
        
        Debug.Log($"[HandlePlayerHit] 내 클라이언트에서 호출됨 → {targetId}, {bullet}");

        // 발사 완료 처리
        isFiring = false;

        if (hitMessageText == null) return;

        string result = bullet == BulletType.live
        ? "<color=red>발사된 탄환은 <b>실탄</b>입니다.</color>"
        : "<color=green>발사된 탄환은 <b>공포탄</b>입니다.</color>";

        hitMessageText.text = result;

        StartCoroutine(ClearMessageAfterDelay(2.5f));
    }

    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitMessageText.text = "";
    }
}