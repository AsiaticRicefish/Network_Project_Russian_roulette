using Managers;
using UnityEngine;

namespace LDH_Animation
{
    //총 오브젝트에다가 붙이거나 총 매니저에 붙이기
    public class GunEffectController : MonoBehaviour
    {
        [Header("총구 이펙트 오브젝트")]
        [SerializeField] private GameObject muzzleFlash;
        
        
        public void PlayShootEffect(bool isLiveBullet)
        {
            if (isLiveBullet)
            {
                Manager.Sound.PlayFire();
                //todo: 발사 이팩트를 활성화 or 총기 자체에 애니메이션에 발사를 만들고 이를 실행
                //todo: 애니메이션이면 사운드도 애니메이션에 연동시켜도 됨
            }
            else
            {
                Manager.Sound.PlayBlank();
            }
               
        }
        //
        // //쏘는 소리
        // public void PlayFireSfx()
        // {
        //     Manager.Sound.PlayFire();
        // }
        //
        // public void PlayBlank()
        // {
        //     Manager.Sound.PlayBlank();
        // }
    }
}