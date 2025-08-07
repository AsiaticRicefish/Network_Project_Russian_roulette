using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 상자 클릭 시 해당 주인의 아이템 싱크에게 요청하도록 변경
/// </summary>
public class BoxClickTrigger : MonoBehaviour
{
    private ItemBoxManager _box;

    private void Awake()
    {
        _box = GetComponent<ItemBoxManager>();
    }

    private void OnMouseDown()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            return;

        if (_box == null)
        {
            Debug.LogWarning("[BoxClickTrigger] ItemBoxManager 없음");
            return;
        }

        string ownerNickname = _box.OwnerNickname;
        Debug.Log($"[BoxClickTrigger] 클릭된 상자 Owner: {ownerNickname}");

        // 모든 ItemSync 중, ownerNickname과 일치하는 주인을 찾음
        var sync = FindObjectsOfType<ItemSync>()
            .FirstOrDefault(s => s.MyNickname == ownerNickname);

        if (sync != null)
        {
            Debug.Log($"[BoxClickTrigger] ItemSync 찾음 → MyNickname: {sync.MyNickname}, IsMine: {sync.IsMine()}");
            sync.BoxOpen();
        }
        else
        {
            Debug.LogWarning($"[BoxClickTrigger] ItemSync 찾을 수 없음: {ownerNickname}");
        }
    }
}