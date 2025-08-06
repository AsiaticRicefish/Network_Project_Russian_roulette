using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DesignPattern
{
    /// ===================================================
    /// - MoneBehaviour 기반 싱글톤 패턴
    /// - Instance 접근자를 통해 인스턴스 접근 가능
    /// - 상속한 클래스의 Awkae에서 SingletonInit() 호출이 필수적으로 필요
    /// - Release() 수동 해제 가능
    ///
    /// 주의사항
    /// - 반드시  SingletonInit() 반드시 호출!!
    /// - Manager 클래스에 접근자(static) 등록 필요
    /// - @Manager 프리팹에 붙이거나 AddComponent로 초기화 보장해야 함
    /// ===================================================
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        protected static T _instance;

        public static T Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<T>();
                    if(_instance!=null)
                        DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        
        /// <summary>
        /// 싱글톤 초기화 (Awake에서 반드시 호출)
        /// </summary>
        protected void SingletonInit()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
           
            _instance = this as T;
            DontDestroyOnLoad(_instance);
        }
        
        /// <summary>
        /// 싱글톤 수동 해제
        /// </summary>
        public static void Release()
        {
            if (_instance != null)
            {
                Destroy(_instance.gameObject);
                _instance = null;
            }
        }
        
    }
}