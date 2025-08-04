using Cinemachine;
using Managers;
using UnityEngine;

namespace GameCamera
{
    [RequireComponent(typeof(CinemachineVirtualCamera))]
    public class VirtualCam_Base : MonoBehaviour
    {
        [Header("Camera Properties")]
        public string cameraID;
        protected CinemachineVirtualCamera _vcam;

        protected virtual void Awake() => Init();

        /// <summary>
        /// override시 base.Init() 호출 필요
        /// </summary>
        protected virtual void Init()
        {
            _vcam = GetComponent<CinemachineVirtualCamera>();

            if (string.IsNullOrEmpty(cameraID))
            {
                Debug.LogWarning($"[{name}] cameraID가 비어 있습니다.");
                return;
            }

            Manager.Camera.RegisterCamera(cameraID, _vcam);
        }
        
        protected virtual void OnDestroy()
        {
            //if (!string.IsNullOrEmpty(cameraID))
                //Manager.Camera.UnregisterCamera(cameraID);
        }

        /// <summary>
        /// 카메라 활성화 시 호출됨 (CameraManager.PushCamera → Priority = 10 됐을 때)
        /// </summary>
        public virtual void OnActivated()
        {
            // 자식 클래스에서 오버라이드 가능
        }

        /// <summary>
        /// 카메라 비활성화 시 호출됨 (CameraManager.PopCamera → Priority = 0 됐을 때)
        /// </summary>
        public virtual void OnDeactivated()
        {
            // 자식 클래스에서 오버라이드 가능
        }
    }
}