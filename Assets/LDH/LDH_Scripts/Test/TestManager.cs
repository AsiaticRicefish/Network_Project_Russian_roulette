using System;
using DesignPattern;
using UnityEngine;

namespace Test
{
    public class TestManager : Singleton<TestManager>
    {
        //원하는 시점에 싱글톤 초기화 진행
        private void Awake() => SingletonInit();

        public void DebugTest()
        {
            Debug.Log("[테스트 매니저] - DebugTest 호출");
        }
    }
}