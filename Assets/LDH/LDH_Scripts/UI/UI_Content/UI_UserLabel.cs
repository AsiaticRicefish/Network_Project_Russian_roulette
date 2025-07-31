using Photon.Pun;
using System;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class UI_UserLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nicknameText;

        private void OnEnable()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                SetData();
            }
            
        }

        private void SetData()
        {
            _nicknameText.text = PhotonNetwork.LocalPlayer.NickName;
        }
        
    }
}