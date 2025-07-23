using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using Utils;

namespace Managers
{
    public class SoundManager : Singleton<SoundManager>
    {
        public AudioMixer audioMixer;
        private AudioMixerGroup[] _audioMixerGroups;

        private AudioSource[] audioSources = new AudioSource[(int)Define_LDH.Sound.MaxCount];
        //private Dictionary<string, AudioClip> sfxAudioClips = new();   // 캐싱 사용 x
        
        public float BgmVolume { get; private set; }
        public float SfxVolume { get; private set; }

        public const string BgmVolumeKey = "BGMVolume"; //playerprefs, audio mixer parameter로 사용
        public const string SfxVolumeKey = "SFXVolume"; //playerprefs, audio mixer parameter로 사용
        
        private void Awake() => Init();

        private void Init()
        {
            SingletonInit();
            
            Clear();
            
            //audio mixer setting
            if (audioMixer == null)
            {
                audioMixer = Resources.Load<AudioMixer>("Sounds/MasterAudioMixer");
                _audioMixerGroups = audioMixer.FindMatchingGroups("Master");
                
                
                //로컬에 있는 정보를 읽어와서 bgm, sfx 볼륨 설정하기
                InitVolume();
            }
            
            
            //SFX, BGM audio source 생성 및 output audio mixer group 지정
            GameObject soundRoot = new GameObject("@Sound");
            // soundRoot.transform.SetParent(Manager.manager.transform);  //dont destroy 처리
            DontDestroyOnLoad(soundRoot);
            
            
            string[] soundTypeNames = System.Enum.GetNames((typeof(Define_LDH.Sound)));
            
            for(int i=0; i<soundTypeNames.Length - 1 ; i++)
            {
                AudioSource audioSource = new GameObject(soundTypeNames[i]).AddComponent<AudioSource>();
                audioSource.outputAudioMixerGroup = _audioMixerGroups[i+1]; //오디오 소스의 output을 할당
                
                audioSources[i] = audioSource;
                audioSource.transform.SetParent(soundRoot.transform);
            }
            
            Debug.Log($"{GetType()} @Sound 생성 및 Bgm, Sfx audio source 생성");
            
            
            //bgm 연속 재생 설정
            audioSources[(int)Define_LDH.Sound.Bgm].loop = true;



        }

        #region Initialize

        /// <summary>
        /// 로컬에 저장된 볼륨 값에 대한 데이터를 가져오고, 있으면 그 값으로, 없으면 전체 데이터로 초기화
        /// </summary>
        private void InitVolume()
        {
            Debug.Log($"[{GetType()}] 저장된 사운드 볼륨 데이터를 가져옵니다.");
            SetBgmVolume((PlayerPrefs.HasKey(BgmVolumeKey)) ? PlayerPrefs.GetFloat(BgmVolumeKey) : 0.5f);
            SetSfxVolume((PlayerPrefs.HasKey(SfxVolumeKey)) ? PlayerPrefs.GetFloat(SfxVolumeKey) : 0.5f);
        }
        
        //모든 오디오 소스의 클립을 빼고 플레이 중단, sfxAudioClip cashing 초기화
        public void Clear()
        {
            foreach (AudioSource audioSource in audioSources)
            {
                if (audioSource != null)
                {
                    audioSource.clip = null;
                    audioSource.Stop();
                }
            }
            //sfxAudioClips.Clear();
        }

        #endregion


        #region Volume Setting
        

        //bgm 볼륨 설정
        //0~1 사이의 값을 믹서 값에 맞게 변환 -> 오디오 믹서에 반영 -> 0~1 사이의 값을 로컬에 저장
        public void SetBgmVolume(float value)
        {
            //볼륨 clamp
            BgmVolume = Mathf.Clamp(value, 0.0001f, 1f);
            
            // 값 전환
            float bgmMixerValue = ValueChange(value);
            
            //오디오 믹서 설정
            audioMixer.SetFloat(BgmVolumeKey, bgmMixerValue);
            
            //로컬 저장
            PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);  //mixer 값이 아닌 0~1 사이의 값 저장
        }
        
        //sfx 볼륨 설정
        //0~1 사이의 값을 믹서 값에 맞게 변환 -> 오디오 믹서에 반영 -> 0~1 사이의 값을 로컬에 저장
        public void SetSfxVolume(float value)
        {
            SfxVolume = Mathf.Clamp(value, 0.0001f, 1f);
            float sfxMixerValue = ValueChange(value);
            audioMixer.SetFloat(SfxVolumeKey, sfxMixerValue);
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        }

        private float ValueChange(float value)
        {
            return Mathf.Log10(Mathf.Clamp(value, 0.0001f, 1f)) * 20f;
        }


        #endregion


        #region Audio Play / Audio Clip
        
        //매개변수로 전닮받은 오디오 클립을 재생
        //bgm은 루프로 계속 재생 하며, sfx는 중첩 가능하도록 play one shot으로 재생한다.
        public void Play(AudioClip audioClip, Define_LDH.Sound soundType = Define_LDH.Sound.Sfx)
        {
            if (audioClip == null)
            {
                Debug.LogWarning($"{GetType()} audio clip이 없습니다. 재생할 수 없습니다.");
                return;
            }
            
            AudioSource audioSource = audioSources[(int)soundType];

            if (soundType == Define_LDH.Sound.Bgm)
            {
                if (audioSource.isPlaying)
                {
                    // 같은 audio clip을 재생하는 경우는 무시
                    if (audioSource.clip == audioClip) return;
                    audioSource.Stop();
                }
                
                // audioclip 교체
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else if (soundType == Define_LDH.Sound.Sfx)
            {
                audioSource.PlayOneShot(audioClip);
            }

            else
            {
                Debug.LogWarning($"[{GetType()}] 잘못된 sound type입니다.");
            }
        }

        #endregion



    }
}

