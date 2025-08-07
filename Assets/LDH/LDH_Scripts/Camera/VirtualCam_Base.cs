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

        private void Start()
        {
            Manager.Camera.PushCamera(cameraID);
        }

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
            _vcam.Priority = 0; //초기화
        }
        
        protected virtual void OnDestroy()
        {
            if (!string.IsNullOrEmpty(cameraID))
                Manager.Camera.UnregisterCamera(cameraID);
        }

        
        
    }
}