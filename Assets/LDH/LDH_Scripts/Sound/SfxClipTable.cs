using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    /// <summary>
    /// 키-클립 매핑 구조로 구성된 SFX 테이블.
    /// ScriptableObject로 저장되며, 키를 통해 AudioClip을 조회할 수 있음.
    /// </summary>
    [CreateAssetMenu(menuName = "Sound/SfxClipTable", order = 0)]
    public class SfxClipTable : ScriptableObject
    {
        /// <summary>
        /// 키(key)에 대응하는 AudioClip을 저장하는 구조체
        /// </summary>
        [System.Serializable]
        public class SfxEntry
        {
            public string key;           // SFX를 식별하는 문자열 키
            public AudioClip clip;       // 실제 재생할 AudioClip
        }
        
        [SerializeField] private List<SfxEntry> _clips;          // Unity Inspector에서 설정할 SFX 목록
        private Dictionary<string, AudioClip> _sfxClipDict;     // 런타임에서 접근하는 내부 Dictionary (key → AudioClip)


        /// <summary>
        /// 내부 Dictionary 초기화.
        /// </summary>
        public void Init()
        {
            if (_sfxClipDict == null)
            {
                _sfxClipDict = new();

                foreach (var sfxEntry in _clips)
                {
                    _sfxClipDict[sfxEntry.key] = sfxEntry.clip;
                }
            }
        }

        /// <summary>
        /// 키를 통해 AudioClip을 조회
        /// 존재하지 않으면 null 반환
        /// </summary>
        /// <param name="key">SFX 식별 키</param>
        /// <returns>AudioClip 또는 null</returns>
        public AudioClip GetAudioClip(string key)
        {
            if (_sfxClipDict == null) Init();

            return _sfxClipDict.TryGetValue(key, out var clip) ? clip : null;
        }
        
    }
}