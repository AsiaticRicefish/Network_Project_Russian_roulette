using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Managers;
using Photon.Pun;
using System;

public class GunController : MonoBehaviourPun
{
    private Animator _animator;
    private bool _isHold = false;
    public event Action<bool> OnHolded;

    [SerializeField] private GameObject _targetSelectUI;

    void Awake()
    {
        _animator = GetComponent<Animator>();
    }
    
    void OnMouseDown()
    {
        if (!_isHold)
        {
            _isHold = true;
            OnHolded?.Invoke(_isHold);
        }
    }
}
