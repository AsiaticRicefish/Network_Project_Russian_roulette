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
    }
}