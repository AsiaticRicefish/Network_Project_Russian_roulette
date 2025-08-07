using Cinemachine;
using DesignPattern;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Utils;

public class CameraManager : Singleton<CameraManager>
{
    private Dictionary<string, CinemachineVirtualCamera> _cameraDict = new();
    private Stack<string> _cameraStack = new();
    private Camera _mainCam;

    private void Awake() => SingletonInit();
    private void Start() => _mainCam = Camera.main;

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;       
    }
    
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log($"[CameraManager] OnSceneLoaded() → 스택 초기화. 씬: {scene.name}");
        ClearStack();  // 스택 클리어
    }


    #region Camera Main Reference
    
    
    /// <summary>
    /// 현재 씬의 MainCamera를 찾아 재지정합니다.
    /// </summary>
    public void UpdateMainCamera()
    {
        _mainCam = Camera.main;
        Debug.Log("[CameraManager] UpdateMainCamera() 호출됨 → MainCamera 갱신");

    }
    
    /// <summary>
    /// 외부에서 직접 MainCamera를 지정합니다.
    /// </summary>
    public void SetMainCamera(Camera cam)
    {
        _mainCam = cam;
        Debug.Log($"[CameraManager] SetMainCamera() → 수동 지정됨: {_mainCam.name}");

    }
    
    #endregion

    #region Camera Register / UnRegister

    /// <summary>
    /// VirtualCamera를 ID와 함께 등록합니다.
    /// </summary>
    public void RegisterCamera(string id, CinemachineVirtualCamera cam)
    {
        if (!string.IsNullOrEmpty(id) && cam != null)
        {
            if (!_cameraDict.ContainsKey(id))
            {
                _cameraDict.Add(id, cam);
                Debug.Log($"[CameraManager] RegisterCamera() → 카메라 등록: {id}");
            }
            else
            {
                Debug.LogWarning($"[CameraManager] RegisterCamera() → 이미 존재하는 ID: {id}");
            }
        }
           
    }
    
    /// <summary>
    /// ID로 등록된 VirtualCamera를 해제합니다.
    /// </summary>
    public void UnregisterCamera(string id)
    {
        if (_cameraDict.ContainsKey(id))
            _cameraDict.Remove(id);
        Debug.Log($"[CameraManager] UnregisterCamera() → 카메라 해제: {id}");

    }
    
    #endregion

    /// <summary>
    /// ID에 해당하는 VirtualCamera를 반환합니다.
    /// </summary>
    public CinemachineVirtualCamera GetCamera(string id)
    {
        _cameraDict.TryGetValue(id, out var cam);
        return cam;
    }
    
    #region Stack-based Camera Switching

    /// <summary>
    /// 지정한 카메라를 스택 최상단에 올리고 우선순위 10으로 설정합니다.
    /// </summary>
    public void PushCamera(string id)
    {
        Debug.Log($"[CameraManager] PushCamera() 호출됨: {id}");

        if (!_cameraDict.ContainsKey(id))
        {
            Debug.LogWarning($"[CameraManager] PushCamera() → {id} 카메라가 존재하지 않음");
            return;
        }

        // 현재 top 비활성화
        if (_cameraStack.Count > 0)
        {
            var prevCam = GetCamera(_cameraStack.Peek());
            if (prevCam != null)
            {
                prevCam.Priority = 0;
                Debug.Log($"[CameraManager] PushCamera() → 이전 카메라 Priority 0 설정: {_cameraStack.Peek()}");
            }
        }
        
        _cameraStack.Push(id);
        GetCamera(id).Priority = 10;
        
        Debug.Log($"[CameraManager] PushCamera() 완료 → 현재 활성 카메라 ID: {id}, 스택 수: {_cameraStack.Count}");
    }

    /// <summary>
    /// 스택에서 현재 카메라를 제거하고, 바로 아래 카메라를 활성화합니다.
    /// </summary>
    public void PopCamera()
    {
        Debug.Log("[CameraManager] PopCamera() 호출됨");

        
        if (_cameraStack.Count == 0)
        {
            Debug.LogWarning("[CameraManager] PopCamera() → 카메라 스택이 비어 있음");
            return;
        }
        
        var poppedId = _cameraStack.Pop();
        var poppedCam = GetCamera(poppedId);
        if (poppedCam != null) poppedCam.Priority = 0;
        Debug.Log($"[CameraManager] PopCamera() → 제거된 카메라 ID: {poppedId}");

        
        if (_cameraStack.Count > 0)
        {
            var newTopId = _cameraStack.Peek();
            var newTopCam = GetCamera(newTopId);
            if (newTopCam != null) newTopCam.Priority = 10;

            Debug.Log($"[CameraManager] PopCamera() → 새 활성 카메라 ID: {newTopId}");

        }
    }
    
    /// <summary>
    /// 모든 카메라 우선순위를 0으로 초기화하고 스택을 비웁니다.
    /// </summary>
    public void ClearStack()
    {
        Debug.Log("[CameraManager] ClearStack() 호출됨");
        foreach (var cam in _cameraDict.Values)
            cam.Priority = 0;
        _cameraStack.Clear();
    }

    #endregion

    #region Impulse (Shake) 기능

    /// <summary>
    /// 현재 스택 최상단 카메라의 ImpulseSource에 흔들림 효과를 줌
    /// </summary>
    /// <param name="force">세기 (기본값 1f)</param>
    public void PlayImpulse(float force = 1f, CinemachineImpulseDefinition.ImpulseShapes impulseShapes = CinemachineImpulseDefinition.ImpulseShapes.Bump)
    {
        Debug.Log("[CameraManager] PlayImpulse() 호출됨");

        if (_cameraStack.Count == 0)
        {
            Debug.LogWarning("[CameraManager] PlayImpulse() → 카메라 스택이 비어 있음");
            return;
        }

        Debug.Log("[CameraManager] 1");
        var camId = _cameraStack.Peek();

        Debug.Log("[CameraManager] 2");
        var cam = GetCamera(camId);

        Debug.Log("[CameraManager] 3");
        if (cam == null)
        {
            Debug.LogWarning($"[CameraManager] PlayImpulse() → {camId} 카메라 없음");
            return;
        }
        Debug.Log("[CameraManager] 4");
        var impulse = cam.GetComponent<CinemachineImpulseSource>();

        Debug.Log("[CameraManager] 5");
        if (impulse == null)
        {
            Debug.LogWarning($"[CameraManager] PlayImpulse() → {camId}에 ImpulseSource 없음");
            return;
        }

        Debug.Log("[CameraManager] Impluse Shape 적용");
        impulse.m_ImpulseDefinition.m_ImpulseShape = impulseShapes;

        impulse.GenerateImpulseWithForce(force);
        Debug.Log($"[CameraManager] PlayImpulse() → {camId}에 force {force}로 Impulse 발생");

    }

    /// <summary>
    /// 특정 카메라 ID에 ImpulseSource가 있으면 흔들림 발생
    /// </summary>
    public void PlayImpulse(string id, float force = 1f)
    {
        Debug.Log($"[CameraManager] PlayImpulse({id}) 호출됨");

        var cam = GetCamera(id);
        if (cam == null)
        {
            Debug.LogWarning($"[CameraManager] PlayImpulse({id}) → 카메라 없음");
            return;
        }

        var impulse = cam.GetComponent<CinemachineImpulseSource>();
        if (impulse == null)
        {
            Debug.LogWarning($"[CameraManager] PlayImpulse({id}) → ImpulseSource 없음");
            return;
        }

        impulse.GenerateImpulseWithForce(force);
        Debug.Log($"[CameraManager] PlayImpulse({id}) → force {force}로 Impulse 발생");

    }

    #endregion
    
    public void ApplyBlenderSettingToBrain(Camera camera, 
        CinemachineBlendDefinition.Style style = CinemachineBlendDefinition.Style.EaseInOut, 
        float blendTime = 0.2f)
    {
        var brain = Util_LDH.GetOrAddComponent<CinemachineBrain>(camera.gameObject);
        brain.m_DefaultBlend = new CinemachineBlendDefinition(style, blendTime);
    }


    public void ApplyBlenderSettingToBrain(Camera camera, string assetName)
    {
        var brain = Util_LDH.GetOrAddComponent<CinemachineBrain>(camera.gameObject);
        
        // Resources에서 BlenderSetting 불러오기
        var blenderSetting = Resources.Load<CinemachineBlenderSettings>($"Data/{assetName}");

        if (blenderSetting == null)
        {
            Debug.LogWarning($"[CameraManager] BlenderSetting '{assetName}' 를 찾을 수 없습니다.");
            return;
        }
        brain.m_CustomBlends = blenderSetting;
        // 실제로 잘 적용됐는지 확인용 디버그 로그
        Debug.Log($"[CameraManager] ✅ BlenderSetting '{assetName}' 적용 완료. 포함된 커스텀 블렌드 수: {blenderSetting.m_CustomBlends?.Length}");

        if (blenderSetting.m_CustomBlends != null)
        {
            foreach (var blend in blenderSetting.m_CustomBlends)
            {
                Debug.Log($"[CameraManager] ↪ From: '{blend.m_From}' → To: '{blend.m_To}' | Style: {blend.m_Blend.m_Style}, Time: {blend.m_Blend.m_Time}");
            }
        }
    }

}