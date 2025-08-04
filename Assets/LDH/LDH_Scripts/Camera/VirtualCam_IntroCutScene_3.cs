using Managers;
using UnityEngine;

namespace GameCamera
{
    public class VirtualCam_IntroCutScene_3 : VirtualCam_Base
    {
        public void PlayImpulse()
        {
            Manager.Camera.PlayImpulse("vc3");
        }
    }
}