using Unity.VisualScripting;
using UnityEngine;

namespace Utils
{
    public class Define_LDH : MonoBehaviour
    {
        public const string NicknameDelimiter = "__";
        
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
        
        public enum GlobalUI
        {
            UI_Setting,
            UI_InventoryInfo,
            UI_Shortcut,
        }
        
        public enum NotifyType
        {
            Error,
            Help,
            Notify,
            Check,
        }
        
        public enum NotifyMessageType
        {
            SignupSuccess,
            SignupError,
            EmailCheckSuccess,
            EmailCheckError,
            LoginSuccess,
            LoginError,
            NicknameError,
            NicknameSuccess,
            CreateRoomError,
            CreeateRoomSuccess,
            RoomCodeError,
            RoomCodeEmpty,
            RoomCodeSuccess,
            JoinRoomError,
            
            
            
        }
    }
}