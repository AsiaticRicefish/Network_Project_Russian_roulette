using Managers;
using Photon.Pun;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Utils;

namespace ETC
{
    public class CutSceneController : MonoBehaviour
    {
        [SerializeField] private PlayableDirector _director;
        [SerializeField] private string GameScene;
        
        private void Awake() => Init();

        private void Init()
        {
            PhotonNetwork.AutomaticallySyncScene = false;
            StopBGM();
            SetAudioSource();
        }

        private void StopBGM()
        {
            Manager.Sound.Stop(Define_LDH.Sound.Bgm);
        }

        public void SetAudioSource()
        {
            // PlayableDirector가 참조하는 TimelineAsset을 가져온다
            TimelineAsset timeline = _director.playableAsset as TimelineAsset;

            if (timeline == null)
            {
                Debug.Log("[CutSceneController] PlayableDirector에 TimelineAsset이 연결되어 있지 않음");
                return;
            }

            // BGM / SFX AudioSource 준비
            var bgmSource = Managers.SoundManager.Instance.GetAudioSource(Define_LDH.Sound.Bgm);
            var sfxSource = Managers.SoundManager.Instance.GetAudioSource(Define_LDH.Sound.Sfx);


            // TimelineAsset에 정의된 모든 Track을 순회
            foreach (var track in timeline.GetOutputTracks())
            {
                if (track is AudioTrack audioTrack)
                {
                    if (audioTrack.name == "BGM Track")
                    {
                        _director.SetGenericBinding(audioTrack, bgmSource);
                        Debug.Log("[TimelineAudioBinder] BGM Track 바인딩 완료");
                    }
                    else if (audioTrack.name == "SFX Track")
                    {
                        _director.SetGenericBinding(audioTrack, sfxSource);
                        Debug.Log("[TimelineAudioBinder] SFX Track 바인딩 완료");
                    }
                }
            }
        }


        public void LoadInGameScene()
        {
            StartCoroutine(Util_LDH.LoadSceneWithDelay(GameScene, 1f));
        }
        
                
        
        
    
    }
}