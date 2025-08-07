using GameUI;
using Managers;
using System;
using UnityEngine;
using Utils;

namespace Test
{
    public class UITest : MonoBehaviour
    {
        [SerializeField] private Transform ui;

        private Camera _camera;
        private void Start()
        {
            _camera = Camera.main;
        }

        private void LateUpdate()
        {
            ui.transform.forward = _camera.transform.forward;
        }
    }
}