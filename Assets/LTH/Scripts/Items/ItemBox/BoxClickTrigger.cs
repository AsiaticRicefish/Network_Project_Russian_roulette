using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoxClickTrigger : MonoBehaviour
{
    private void OnMouseDown()
    {
        var itemSync = FindObjectOfType<ItemSync>();

        // 혹시 myItemBox가 null이라면 즉석에서 연결
        if (itemSync != null && itemSync.myItemBox == null)
        {
            var boxes = FindObjectsOfType<ItemBoxManager>();
            foreach (var box in boxes)
            {
                if (box.OwnerId == PhotonNetwork.LocalPlayer.NickName)
                {
                    itemSync.Init(box.OwnerId, box);
                    Debug.Log("[BoxClickTrigger] myItemBox 즉시 연결 완료");
                    break;
                }
            }

            if (itemSync.myItemBox == null)
            {
                Debug.LogError("[BoxClickTrigger] Init 실패! 상자 연결에 실패했습니다.");
                return;
            }
        }

        itemSync?.BoxOpen(); // null 체크
    }
}