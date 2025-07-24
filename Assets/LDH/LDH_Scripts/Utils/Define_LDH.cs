using UnityEngine;

namespace Utils
{
    public class Define_LDH : MonoBehaviour
    {
        // 사운드 타입
        public enum Sound
        {
            Bgm,
            Sfx,
            MaxCount,
        }
        
        public enum UIEvent
        {
            Click,
            PointEnter,
            PointExit,
            Drag,
        }

    }
}