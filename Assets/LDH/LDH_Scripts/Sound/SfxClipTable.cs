using System.Collections.Generic;
using UnityEngine;

namespace Sound
{
    [CreateAssetMenu(menuName = "Sound/SfxClipTable", order = 0)]
    public class SfxClipTable : ScriptableObject
    {
        [System.Serializable]
        public class SfxEntry
        {
            public string key;
            public AudioClip clip;
        }
        
        public List<SfxEntry> clips;
        private Dictionary<string, AudioClip> _sfxClipDict;

        public void Init()
        {
            if (_sfxClipDict == null)
            {
                _sfxClipDict = new();

                foreach (var sfxEntry in clips)
                {
                    _sfxClipDict[sfxEntry.key] = sfxEntry.clip;
                }
            }
        }

        public AudioClip GetAudioClip(string key)
        {
            if (_sfxClipDict == null) Init();

            return _sfxClipDict.TryGetValue(key, out var clip) ? clip : null;
        }
        
    }
}