using Michsky.UI.ModernUIPack;
using System;
using TMPro;
using UnityEngine;

namespace GameUI
{
    public class UI_Nickname : MonoBehaviour
    {
        [Header("Manager")]
        [SerializeField] private ModalWindowManager _modalWindowManager;
        
        [Header("UI Elements")] 
        [SerializeField] private TMP_InputField _nicknameField;

        
        public void SetActive(bool isActive)
        {
            if (isActive)
            {
                Debug.Log("nickname panel set actvid true 호출");
                _modalWindowManager.OpenWindow();
                
            }
            else
                _modalWindowManager.CloseWindow();
        }
    }
}