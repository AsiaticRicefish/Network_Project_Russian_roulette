using Cinemachine;
using Managers;
using System.Collections;
using UnityEngine;

namespace GameCamera
{
    public class VirtualCam_BulletDisplay : VirtualCam_Base
    {
        protected override void Awake()
        {
            cameraID = "BulletDisplay";
            //base.Awake();
        }
        
        public IEnumerator RegisterVCam()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();

            if (string.IsNullOrEmpty(cameraID))
            {
                Debug.LogWarning($"[{name}] cameraID가 비어 있습니다.");
                yield break;
            }
            
            Manager.Camera.RegisterCamera(cameraID, _vcam);
            _vcam.Priority = 0; //초기화

            yield return null;
        }
        
    }
}