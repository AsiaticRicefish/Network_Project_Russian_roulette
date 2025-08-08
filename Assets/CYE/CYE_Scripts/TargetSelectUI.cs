using GameUI;
using Photon.Pun;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

public class TargetSelectUI : MonoBehaviour
{
    [SerializeField] private GameObject[] _targetButtonArray = new GameObject[2];
    public FireSync _fireSync;
    public GunController _gunController;
    
    private Color originColor = Color.white;
    private Color hoverColor = Color.red;

    private void Start()
    {
        
        foreach (GameObject button in _targetButtonArray)
        {
            UI_Base.BindUIEvent(button, (_) =>
            {
                ChangeTextColor(button, Color.red);
            }, Define_LDH.UIEvent.PointEnter);
            
            UI_Base.BindUIEvent(button, (_) =>
            {
                ChangeTextColor(button, Color.white);
            }, Define_LDH.UIEvent.PointExit);
            
        }
   
    }

    private void OnEnable()
    {
        InitializeNameTag();
    }

    private void OnDisable()
    {
        foreach (GameObject buttonObj in _targetButtonArray)
        {
            buttonObj.GetComponentInChildren<TMP_Text>().color = originColor;
        }
    }

    public void SetGunController(GunController gunController)
    {
        _gunController = gunController;
        gameObject.SetActive(false);
    }

    public void InitializeNameTag()
    {
        foreach (Photon.Realtime.Player p in PhotonNetwork.PlayerList)
        {
            if (p == PhotonNetwork.LocalPlayer)
            {
                _targetButtonArray[1].GetComponent<TargetSelectButton>()?.SetTargetId(p.NickName, p.NickName);
            }
            else
            {
                _targetButtonArray[0].GetComponent<TargetSelectButton>()?.SetTargetId(p.NickName, p.NickName);
            }
        }
    }

    private void ChangeTextColor(GameObject buttonObj, Color color)
    {
        buttonObj.GetComponentInChildren<TMP_Text>().color = color;
    }
}
