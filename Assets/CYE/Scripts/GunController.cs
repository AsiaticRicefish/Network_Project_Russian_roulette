using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;

public class GunController : MonoBehaviour
{
    private Animator _animator;
    void Awake()
    {
        // _animator = GetComponent<Animator>();
        _animator = GetComponentInChildren<Animator>();
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        { 
            // target 지정을 어떻게 할 것인가? 어디서 저장하여 불러올 것인가?
            // Manager.Gun.Fire(target);
        }
    }
}
