using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TargetSelectButton : MonoBehaviour
{
    private Button _btnComponent;
    private string _userId;
    public string UserId { get { return _userId; } set { _userId = value; } }

    void Start()
    {
        _btnComponent = GetComponent<Button>();
        _btnComponent.onClick.AddListener(SelectTarget);
    }

    public void SelectTarget()
    {
        Managers.Manager.Gun.Fire(PhotonNetwork.LocalPlayer.UserId, _userId);
    }
}
