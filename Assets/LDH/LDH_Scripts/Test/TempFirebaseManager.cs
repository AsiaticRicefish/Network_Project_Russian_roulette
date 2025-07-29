using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Test
{
    public class TempFirebaseManager : MonoBehaviour
    {
        public event Action<bool> OnCheckEmailResult;
        
        [SerializeField] private float _responseDelay = 1.5f;
        [SerializeField] private bool _testResult = true;

        public void SimulateEmailCheck()
        {
            StartCoroutine(FakeEmailCheckCoroutine());
        }
        private IEnumerator FakeEmailCheckCoroutine()
        {
            yield return new WaitForSeconds(_responseDelay);
            OnCheckEmailResult?.Invoke(_testResult);
        }

        
    }
}