using System.Collections;
using System.Collections.Generic;
using DesignPattern;
using Sound;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Events;
using UnityEngine.Rendering.VirtualTexturing;
using Utils;

namespace Managers
{
    /// <summary>
    /// 전체 게임에서 사운드를 제어하는 싱글톤 매니저.
    /// BGM 및 SFX 재생, 볼륨 조절(볼륨 데이터 로컬 저장) 등의 기능을 제공한다.
    /// </summary>
    public class SoundManager : Singleton<SoundManager>
    {
        [Header("AudioMixer / AudioMizerGroups")] 
        public AudioMixer audioMixer;                        // 전체 오디오 믹서를 관리하는 AudioMixer
        private AudioMixerGroup[] _audioMixerGroups;        // Master 오디오 믹서 하위의 AudioMixerGroup, 사운드 타입별로 분리(순서대로 BGM, SFX 사운드)

        [Header("AudioSources")]
        private AudioSource[] _audioSources = new AudioSource[(int)Define_LDH.Sound.MaxCount];      // 사운드 타입별 AudioSource 배열
        
        [Header("SFX Mapping Table")]
        private SfxClipTable _sfxClipTable;                 // SFX 키와 AudioClip을 매핑한 테이블 (ScriptableObject)
        [SerializeField] private string _sfxClipTablePath = "Sounds/SfxClipData";        // SfxClipTable을 불러올 리소스 경로
        
        //private Dictionary<string, AudioClip> sfxAudioClips = new();   // 캐싱 사용 x
        
        [Header("Volume Control")]
        public const string BgmVolumeKey = "BGMVolume";         // PlayerPrefs에 저장될 BGM 볼륨 키 및 AudioMixer 파라미터 이름
        public const string SfxVolumeKey = "SFXVolume";         // PlayerPrefs에 저장될 SFX 볼륨 키 및 AudioMixer 파라미터 이름
        [field: SerializeField] public float BgmVolume { get; private set; }     // 현재 설정된 BGM 볼륨 (0~1)
        [field: SerializeField] public float SfxVolume { get; private set; }             // 현재 설정된 SFX 볼륨 (0~1)
        
        private void Awake() => Init();

        
        /// <summary>
        /// SoundManager 초기화.
        ///  - 싱글톤 설정
        /// - AudioMixer 및 AudioSource 구성
        /// - SFX 테이블 로드
        /// - 볼륨 설정 초기화
        /// </summary>
        private void Init()
        {
            SingletonInit();
            Clear();
            InitializeMixer();
            InitializeAudioSources();
            LoadSfxClipTable();
            StartCoroutine(LoadSavedVolumes());

        }

        #region Initialize
        
        /// <summary>
        /// MasterAudioMixer를 리소스에서 불러오고, 하위 그룹들을 배열로 저장
        /// </summary>
        private void InitializeMixer()
        {
            if (audioMixer == null)
                audioMixer = Resources.Load<AudioMixer>("Sounds/MasterAudioMixer");

            _audioMixerGroups = audioMixer.FindMatchingGroups("Master");
        }
        
        /// <summary>
        /// 사운드 타입(BGM, SFX 등)별 AudioSource를 생성하고 Mixer 그룹에 연결.
        /// 생성한 AudioSource는 @Sound 오브젝트(루트 오브젝트) 아래에 배치
        /// 루트 오브젝트는 DontDestroy 처리하여 씬 전환 시에도 유지
        /// </summary>
        private void InitializeAudioSources()
        {
            //SFX, BGM audio source 생성 및 output audio mixer group 지정
            GameObject soundRoot = new GameObject("@Sound");
            DontDestroyOnLoad(soundRoot);

            string[] soundTypeNames = System.Enum.GetNames(typeof(Define_LDH.Sound));

            for (int i = 0; i < soundTypeNames.Length - 1; i++)
            {
                AudioSource source = new GameObject(soundTypeNames[i]).AddComponent<AudioSource>();
                source.outputAudioMixerGroup = _audioMixerGroups[i + 1];  // +1: Master 제외
                source.transform.SetParent(soundRoot.transform);
                _audioSources[i] = source;
            }

            _audioSources[(int)Define_LDH.Sound.Bgm].loop = true;

            Debug.Log($"[{GetType().Name}] @Sound 생성 및 Bgm, Sfx audio source 생성과 초기화");
        }
        
        /// <summary>
        /// SfxClipTable을 리소스 경로에서 로드하고 내부 Dictionary로 초기화
        /// </summary>
        private void LoadSfxClipTable()
        {
            _sfxClipTable = Resources.Load<SfxClipTable>(_sfxClipTablePath);
            _sfxClipTable.Init();
        }

        /// <summary>
        /// PlayerPrefs에 저장된 BGM, SFX 볼륨 값을 가져와 설정. 없을 경우 기본값 0.5f를 사용
        /// </summary>
        private IEnumerator LoadSavedVolumes()
        {
            yield return null;
            SetBgmVolume((PlayerPrefs.HasKey(BgmVolumeKey)) ? PlayerPrefs.GetFloat(BgmVolumeKey) : 50f);
            SetSfxVolume((PlayerPrefs.HasKey(SfxVolumeKey)) ? PlayerPrefs.GetFloat(SfxVolumeKey) : 50f);
            
        }
        
        #endregion


        #region Volume Control
        

        /// <summary>
        /// BGM 볼륨을 0~1 범위로 설정하고 AudioMixer에 반영하며 로컬에 저장
        /// </summary>
        public void SetBgmVolume(float value)
        {
            //볼륨 clamp
            BgmVolume = ClampVolume(value);
            
            //오디오 믹서 설정
            audioMixer.SetFloat(BgmVolumeKey, ToDecibel(BgmVolume));
            
            //로컬 저장
            PlayerPrefs.SetFloat(BgmVolumeKey, BgmVolume);  //mixer 값이 아닌 0~1 사이의 값 저장
        }
        
        /// <summary>
        /// SFX 볼륨을 0~1 범위로 설정하고 AudioMixer에 반영하며 로컬에 저장
        /// </summary>
        public void SetSfxVolume(float value)
        {
            SfxVolume = ClampVolume(value);
            audioMixer.SetFloat(SfxVolumeKey, ToDecibel(SfxVolume));
            PlayerPrefs.SetFloat(SfxVolumeKey, SfxVolume);
        }
        
        /// <summary>
        /// 볼륨 입력값을 0.0001 ~ 1.0 범위로 클램핑
        /// </summary>
        private float ClampVolume(float value) => Mathf.Clamp(value, 0.0001f, 100f);

        /// <summary>
        /// 0~1 범위의 값을 데시벨(-80~0) 값으로 변환
        /// </summary>
        private float ToDecibel(float value)
        {
            float volume = Mathf.Max(value / 100f, 0.0001f);
            return Mathf.Log10(volume) * 20f;
        }



        #endregion


        #region Audio Play / Audio Clip
        
        /// <summary>
        /// 지정된 AudioClip을 재생.
        /// BGM은 루프 재생, 동일 클립 중복 방지.
        /// SFX는 PlayOneShot으로 중첩 재생 가능.
        /// </summary>
        public void Play(AudioClip audioClip, Define_LDH.Sound soundType = Define_LDH.Sound.Sfx)
        {
            if (audioClip == null)
            {
                Debug.LogWarning($"[{GetType().Name}] 재생할 오디오 클립이 없습니다.");
                return;
            }
            
            AudioSource audioSource = _audioSources[(int)soundType];

            if (soundType == Define_LDH.Sound.Bgm)
            {
                // 같은 audio clip을 재생하는 경우는 무시
                if (audioSource.clip == audioClip) return;
                
                // audioclip 교체
                audioSource.Stop();
                audioSource.clip = audioClip;
                audioSource.Play();
            }
            else if (soundType == Define_LDH.Sound.Sfx)
            {
                audioSource.PlayOneShot(audioClip);
            }

            else
            {
                Debug.LogWarning($"[{GetType().Name}] 알 수 없는 사운드 타입입니다: {soundType}");
            }
        }

        /// <summary>
        /// 키 값으로 SFX 테이블에서 오디오 클립을 조회해 재생
        /// </summary>
        public void PlaySfxByKey(string sfxKey)
        {
            var audioClip = _sfxClipTable.GetAudioClip(sfxKey);
            
            if (audioClip == null)
            {
                Debug.LogWarning($"[{GetType().Name}] 키 '{sfxKey}' 에 해당하는 SFX가 없습니다.");
                return;
            }
            
            AudioSource audioSource = _audioSources[(int)Define_LDH.Sound.Sfx];
            audioSource.PlayOneShot(audioClip);
        }


        /// <summary>
        /// 지정한 사운드 타입의 오디오 재생을 중지.
        /// (클립은 유지, 단순 정지만 수행)
        /// </summary>
        public void Stop(Define_LDH.Sound soundType)
        {
            AudioSource source = _audioSources[(int)soundType];

            if (source == null)
            {
                Debug.LogWarning($"[{GetType().Name}] AudioSource가 존재하지 않습니다: {soundType}");
                return;
            }

            source.Stop();
            Debug.Log($"[{GetType().Name}] {soundType} 재생 정지");
        }
        
        #endregion


        #region Utility

        /// <summary>
        /// 모든 오디오 소스를 정지시키고 클립을 제거
        /// </summary>
        public void Clear()
        {
            foreach (var source in _audioSources)
            {
                if (source != null)
                {
                    source.Stop();
                    source.clip = null;
                }
            }
        }
        
        /// <summary>
        /// 사운드 타입(BGM 또는 SFX)에 해당하는 AudioSource를 반환
        /// </summary>
        public AudioSource GetAudioSource(Define_LDH.Sound soundType)
        {
            return _audioSources[(int)soundType];
        }

        #endregion

    }
}

