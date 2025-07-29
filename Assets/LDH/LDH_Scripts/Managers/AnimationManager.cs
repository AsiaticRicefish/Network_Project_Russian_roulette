using DesignPattern;
using UnityEngine;
using DG.Tweening;
using LDH_Animation;

namespace Managers
{
    public class AnimationManager : Singleton<AnimationManager>
    {

        private void Awake() => SingletonInit();
        public AnimationBuilder Create(Transform target)
        {
            return new AnimationBuilder(target);
        }
    }
}