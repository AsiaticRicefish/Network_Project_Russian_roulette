using System;
using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace GameUI
{
    public class UI_VolumeSetting : MonoBehaviour
    {
        [SerializeField] private Slider _bgmSlider;
        [SerializeField] private Slider _sfxSlider;
        [SerializeField] private float _sliderMinVal = 0f;
        [SerializeField] private float _sliderMaxVal = 1f;

        private float epsilon = 0.0001f;

        private void Awake() => Init();

        private void OnEnable()
        {
            //사운드 값 반영
            SetSlider();
            //이벤트 구독 처리
            _bgmSlider.onValueChanged.AddListener(SetBgmVolume);
            _sfxSlider.onValueChanged.AddListener(SetSfxVolume);
        }

        private void OnDisable()
        {
            //이벤트 구독 해제
            _bgmSlider.onValueChanged.RemoveListener(SetBgmVolume);
            _bgmSlider.onValueChanged.RemoveListener(SetSfxVolume);
        }


        #region Initialize

        private void Init()
        {
            _bgmSlider.minValue = _sfxSlider.minValue = _sliderMinVal;
            _bgmSlider.maxValue = _sfxSlider.maxValue = _sliderMaxVal;
        }

        private void SetSlider()
        {
            _bgmSlider.value = Manager.Sound.BgmVolume;
            _sfxSlider.value = Manager.Sound.SfxVolume;
        }

        #endregion

        #region Volume Control

        private void SetBgmVolume(float sliderValue)
        {
            if (Mathf.Abs(sliderValue - Manager.Sound.BgmVolume) > epsilon)
            {
                Manager.Sound.SetBgmVolume(sliderValue);
            }
        }

        private void SetSfxVolume(float sliderValue)
        {
            if (Mathf.Abs(sliderValue - Manager.Sound.SfxVolume) > epsilon)
            {
                Manager.Sound.SetSfxVolume(sliderValue);
            }
        }

        #endregion
    }
}