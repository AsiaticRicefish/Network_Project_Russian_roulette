using Managers;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace Test
{
    public class CameraChangeTest : MonoBehaviour
    {
        private Button button;

        private void Awake()
        {
            button = GetComponent<Button>();
            button.onClick.AddListener(ChangeToBulletVcam);
        }

        private void ChangeToBulletVcam()
        {
            if(Manager.Camera.GetCamera("BulletDisplay").Priority == 0)
                Manager.Camera.PushCamera("BulletDisplay");
            else
                Manager.Camera.PopCamera();
        }
    }
}