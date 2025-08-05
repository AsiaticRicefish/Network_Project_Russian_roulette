using UnityEngine;

namespace GameCamera
{
    public class VirtualCam_BulletDisplay : VirtualCam_Base
    {
        protected override void Awake()
        {
            cameraID = "BulletDisplay";
            base.Awake();
        }
        
    }
}