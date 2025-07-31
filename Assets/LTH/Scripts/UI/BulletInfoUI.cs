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
        myId = PhotonNetwork.NickName; // 현재 내 ID
    }

    private void Update()
    {
        timer += Time.deltaTime;
        if (timer < updateInterval) return;
        timer = 0f;

        var gun = GunManager.Instance;
        if (gun == null) return;

        int liveCount = 0;
        int blankCount = 0;

        foreach (var bullet in gun.Magazine)
        {
            if (bullet == BulletType.live) liveCount++;
            else if (bullet == BulletType.blank) blankCount++;
        }

        string loadedText = gun.LoadedBullet == BulletType.live ? "실탄" :
                            gun.LoadedBullet == BulletType.blank ? "공포탄" : "없음";

        // 턴 정보 가져오기
        string turnInfo = "";
        if (TurnSync.CurrentTurnPlayerId != myId)
        {
            turnInfo = "<color=red> 내 턴이 아닙니다 </color>\n";
        }
        else
        {
            turnInfo = "<color=green> 내 턴입니다</color>\n";
        }


        bulletInfoText.text = turnInfo +
                             // $"장전된 탄: {loadedText}\n" +
                              $"남은 탄 수: {gun.Magazine.Count}\n" +
                              $"[ 실탄: {liveCount} | 공포탄: {blankCount} ]";
    }
}