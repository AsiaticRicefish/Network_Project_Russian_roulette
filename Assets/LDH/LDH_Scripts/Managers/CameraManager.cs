using Cinemachine;
using DesignPattern;
using System.Collections.Generic;
using UnityEngine;

public class CameraManager : Singleton<CameraManager>
{
    private Dictionary<string, CinemachineVirtualCamera> _cameraDict = new();
    private Stack<string> _cameraStack = new();
    private Camera _mainCam;

    private void Awake() => SingletonInit();
    private void Start() => _mainCam = Camera.main;

    #region Camera Register / UnRegister
    
    public void RegisterCamera(string id, CinemachineVirtualCamera cam)
    {
        if (!string.IsNullOrEmpty(id) && cam != null)
        {
            if(!_cameraDict.ContainsKey(id))
                _cameraDict.Add(id, cam);
            else
                Debug.Log($"[{GetType().Name}] 해당 id로 등록된 카메라가 존재합니다.");
        }
           
    }
    
    #endregion

    
    public CinemachineVirtualCamera GetCamera(string id)
    {
        _cameraDict.TryGetValue(id, out var cam);
        return cam;
    }
    
    #region 스택 기반 전환

    public void PushCamera(string id)
    {
        if (!_cameraDict.ContainsKey(id))
        {
            Debug.LogWarning($"[CameraManager] {id} 카메라가 없습니다.");
            return;
        }

        // 현재 top 비활성화
        if (_cameraStack.Count > 0)
        {
            var prevCam = GetCamera(_cameraStack.Peek());
            if (prevCam != null) prevCam.Priority = 0;
        }

        _cameraStack.Push(id);
        GetCamera(id).Priority = 10;
    }

    public void PopCamera()
    {
        if (_cameraStack.Count == 0)
        {
            Debug.LogWarning("[CameraManager] 카메라 스택이 비어있습니다.");
            return;
        }

        var prevId = _cameraStack.Pop();
        var prevCam = GetCamera(prevId);
        if (prevCam != null) prevCam.Priority = 0;

        if (_cameraStack.Count > 0)
        {
            var newTop = GetCamera(_cameraStack.Peek());
            if (newTop != null) newTop.Priority = 10;
        }
    }

    public void ClearStack()
    {
        foreach (var cam in _cameraDict.Values)
            cam.Priority = 0;
        _cameraStack.Clear();
    }

    #endregion
    
    #region Impulse 기능

    /// <summary>
    /// 현재 스택 최상단 카메라의 ImpulseSource에 흔들림 효과를 줌
    /// </summary>
    /// <param name="force">세기 (기본값 1f)</param>
    public void PlayImpulse(float force = 1f)
    {
        if (_cameraStack.Count == 0)
        {
            Debug.LogWarning("[CameraManager] 카메라 스택이 비어 있어 Impulse를 줄 수 없습니다.");
            return;
        }

        var camId = _cameraStack.Peek();
        var cam = GetCamera(camId);
        if (cam == null)
        {
            Debug.LogWarning($"[CameraManager] {camId} 카메라를 찾을 수 없습니다.");
            return;
        }

        var impulse = cam.GetComponent<CinemachineImpulseSource>();
        if (impulse == null)
        {
            Debug.LogWarning($"[CameraManager] {camId} 카메라에 ImpulseSource가 없습니다.");
            return;
        }

        impulse.GenerateImpulseWithForce(force);
    }

    /// <summary>
    /// 특정 카메라 ID에 ImpulseSource가 있으면 흔들림 발생
    /// </summary>
    public void PlayImpulse(string id, float force = 1f)
    {
        var cam = GetCamera(id);
        if (cam == null)
        {
            Debug.LogWarning($"[CameraManager] {id} 카메라를 찾을 수 없습니다.");
            return;
        }

        var impulse = cam.GetComponent<CinemachineImpulseSource>();
        if (impulse == null)
        {
            Debug.LogWarning($"[CameraManager] {id} 카메라에 ImpulseSource가 없습니다.");
            return;
        }

        impulse.GenerateImpulseWithForce(force);
    }

    #endregion
}