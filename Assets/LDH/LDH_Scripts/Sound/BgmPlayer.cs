using Managers;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils;

namespace Sound
{
    /// <summary>
    /// 씬 또는 특정 상황에서 BGM을 재생하는 컴포넌트.
    /// - AudioClip 배열을 받아 인덱스를 기준으로 SoundManager를 통해 BGM을 재생함
    /// - 특정 인덱스를 수동으로 재생하거나, 시작 시 자동 재생할 수 있음
    /// </summary>
    public class BgmPlayer : MonoBehaviour
    {
        [Header("BGM Playlist")]
        [Tooltip("재생 가능한 BGM 목록입니다. 인덱스로 접근하여 재생합니다.")]
        [SerializeField] private AudioClip[] _bgmList;
        
        [Header("Auto Play Setting")]
        [Tooltip("Start 시 BGM 목록의 첫 번째 클립을 자동으로 재생할지 여부")]
        [SerializeField] private bool playFirstBgmOnStart = true;


        /// <summary>
        /// playFirstBgmOnStart가 true로 설정되어 있는 경우, BGM 목록의 첫 번째 클립을 자동으로 재생합니다.
        /// </summary>
        private void Start()
        {
            if (playFirstBgmOnStart) PlayBgm(0);
        }


        /// <summary>
        /// 인덱스를 기반으로 지정된 BGM을 SoundManager를 통해 재생합니다
        /// BGM 클립은 Inspector에서 미리 설정해두고, 외부 스크립트에서 이 메서드를 호출하여 원하는 시점에 BGM을 교체할 수 있습니다.
        ///  자동 재생이 꺼져 있는 경우에도 유용하게 사용할 수 있습니다.
        /// </summary>
        public void PlayBgm(int index)
        {
            if (!Util_LDH.IsValidIndex(index, _bgmList))
            {
                Debug.LogWarning($"[{GetType().Name}] _bgmList가 null이거나 index({index})가 범위를 벗어납니다.");
                return;
            }

            Manager.Sound.Play(_bgmList[index], Define_LDH.Sound.Bgm);
        }
    }
}