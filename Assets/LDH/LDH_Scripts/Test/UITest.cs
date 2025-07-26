using GameUI;
using Managers;
using System;
using UnityEngine;
using Utils;

namespace Test
{
    public class UITest : MonoBehaviour
    {
        private UI_Base settingUI;

        private void Start()
        {
            settingUI = Manager.UI.GetGlobalUI(Define_LDH.GlobalUI.UI_Setting);
        }

        private void Update()
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                if (!settingUI.gameObject.activeSelf)
                {
                    Manager.UI.ShowGlobalUI(Define_LDH.GlobalUI.UI_Setting);
                }
                else
                {
                    Manager.UI.CloseGlobalUI(Define_LDH.GlobalUI.UI_Setting);
                }
            }
        }
    }
}