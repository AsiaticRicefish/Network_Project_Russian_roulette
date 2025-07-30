using Managers;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_GunController : MonoBehaviour
{
    [SerializeField] private Button fireButton;
    [SerializeField] private FireSync fireSync;
    [SerializeField] private TextMeshProUGUI hitMessageText;

    private string myId;

    private void Start()
    {
        myId = PhotonNetwork.NickName;
        fireButton.onClick.AddListener(OnFireButtonClicked);

        // 초기 메시지 비우기
        if (hitMessageText != null)
            hitMessageText.text = "";

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
        // 내 턴일 때만 버튼 활성화
        fireButton.interactable = (TurnSync.CurrentTurnPlayerId == myId);
    }

    private void OnFireButtonClicked()
    {
        if (TurnSync.CurrentTurnPlayerId != myId)
        {
            Debug.LogWarning("[FireButtonController] 내 턴이 아님 → 발사 안 됨");
            return;
        }

        fireSync.photonView.RPC("Fire", RpcTarget.All, myId, (int)GunManager.Instance.LoadedBullet);
    }

    // 맞은 사람과 탄 종류에 따라 메시지 출력
    private void HandlePlayerHit(string targetId, BulletType bullet)
    {
        if (hitMessageText == null) return;

        string result = bullet == BulletType.live
            ? $"<color=red>{targetId}</color> 이(가) <b>실탄</b>에 맞았습니다!"
            : $"<color=green>{targetId}</color> 은(는) <b>공포탄</b>이었습니다.";

        hitMessageText.text = result;

        StartCoroutine(ClearMessageAfterDelay(2.5f));
    }

    private IEnumerator ClearMessageAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        hitMessageText.text = "";
    }
}