using Cinemachine;
using Managers;
using System;
using System.Collections;
using UnityEngine;

namespace GameCamera
{
    public class VirtualCam_LocalPlayer :VirtualCam_Base
    {
        protected override void Awake()
        {
            cameraID = "Player";
            base.Awake();

            
        }

        // private IEnumerator Start()
        // {
        //     // yield return null;
        //     // CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        //     // vcam.enabled = false;
        //     // yield return null;
        //     //
        //     // vcam.enabled = true;
        //     //
        //     // Debug.Log("push 시점");
        //     //
        //     Manager.Camera.PushCamera(cameraID);
        // }
        
    }
}