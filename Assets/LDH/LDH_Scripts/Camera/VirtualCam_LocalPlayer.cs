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

            // Manager.Camera.PushCamera(cameraID);
        }

        // private IEnumerator Start()
        // {
        //     yield return null;
        //     CinemachineVirtualCamera vcam = GetComponent<CinemachineVirtualCamera>();
        //                 
        //     Debug.Log("vam enable false");
        //     vcam.enabled = false;
        //     yield return null;
        //     Debug.Log("vam enable true");
        //     vcam.enabled = true;
        //     
        //     Debug.Log("push 시점");
        //     
        //
        // }
        //
    }
}