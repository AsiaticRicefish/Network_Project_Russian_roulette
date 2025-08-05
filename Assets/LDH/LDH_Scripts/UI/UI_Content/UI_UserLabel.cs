using Photon.Pun;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Utils;

namespace GameUI
{
    public class UI_UserLabel : MonoBehaviour
    {
        [SerializeField] private TMP_Text _nicknameText;

        //디버깅 용을 위해 버튼 기능 추가
        private Button _userLabelBtn;


        private void Awake()
        { 
           _userLabelBtn =  Util_LDH.GetOrAddComponent<Button>(gameObject);
        }

        private void Start()
        {
            _userLabelBtn.onClick.AddListener(PrintUserNickName);
        }

        private void OnDestroy()
        {
            _userLabelBtn.onClick.RemoveAllListeners();
        }

        private void OnEnable()
        {
            if (PhotonNetwork.IsConnectedAndReady)
            {
                SetData();
            }
            
        }

        private void SetData()
        {
            _nicknameText.text = Util_LDH.GetUserNickname(PhotonNetwork.LocalPlayer);
        }


        private void PrintUserNickName()
        {
            string uniqueNickname = PhotonNetwork.LocalPlayer.NickName;
            string userNickname = Util_LDH.GetUserNickname(PhotonNetwork.LocalPlayer);
            Debug.Log($"photon nickname : {uniqueNickname}, user nickname : {userNickname} ");
        }
        
    }
}