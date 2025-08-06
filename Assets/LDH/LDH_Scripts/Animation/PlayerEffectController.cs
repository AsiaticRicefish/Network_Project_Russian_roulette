using DG.Tweening;
using Managers;
using Photon.Pun;
using System.Collections;
using UnityEngine;

namespace LDH_Animation
{
  
    //플레이어 프리팹에게 해당 컴포넌트를 붙이고
    //rpc에서 호출해주기
    public class PlayerEffectController : MonoBehaviourPun
    {
        [SerializeField] private CanvasGroup fadeCanvas;
        [SerializeField] private GameObject renderingBody;
        private float damageEffectTime = 1.5f;
        private float fadeOutTime = 0.5f;
        private float fadeInTime = 0.3f;
        private float delayTime => damageEffectTime - (fadeOutTime + fadeInTime);
        
        //쏘는 효과
        public void PlayShoot()
        {
            //todo: 발사 애니메이션 재생
            //소리를 애니메이션에 추가하는 방식으로 하거나 코드로 재생
            
        }
        
        //맞는 효과 -> 끝날때까지 기다려야하니까 코루틴으로 만들어야 할지 고민
        public void PlayDamaged()
        {
            //내 플레이어면 
            if (photonView.IsMine)
            {   
                //시야 암전 -> 일정 시간 후 시야가 복구하는 코루틴 실행
                StartCoroutine(FadeOutAndRespawn());

            }
            else
            {
                StartCoroutine(DamagedAndRespawn());
            }
        }


        private IEnumerator FadeOutAndRespawn()
        {
            if (fadeCanvas == null) yield break;
            fadeCanvas.alpha = 0f;
            fadeCanvas.gameObject.SetActive(true);
            
            
            // 페이드 인
            fadeCanvas.DOFade(1f, fadeInTime).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(fadeInTime);
            
            yield return new WaitForSeconds(delayTime);
            
            // 페이드 아웃
            fadeCanvas.DOFade(0f, fadeOutTime).SetEase(Ease.OutQuad);
            yield return new WaitForSeconds(fadeOutTime);
            
            fadeCanvas.gameObject.SetActive(false);
            
        }

        private IEnumerator DamagedAndRespawn()
        {
            //todo : 피격 애니메이션 재생하기

            if (renderingBody == null)
            {
                Debug.LogWarning("[PlayerEffectController] 플레이어 body is null");
                yield break;
            }

            yield return new WaitForSeconds(fadeInTime);
            renderingBody?.SetActive(false);
            yield return new WaitForSeconds(delayTime);
            renderingBody?.SetActive(true);
            yield return new WaitForSeconds(fadeInTime);
        }
        

    }
}