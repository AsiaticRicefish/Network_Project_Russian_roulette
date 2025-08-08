using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace Utils
{
    public class Define_LDH : MonoBehaviour
    {
        public const string NicknameDelimiter = "__";

        private static Dictionary<SceneName, string> sceneNameDict = new()
        {
            [SceneName.Title] = "Title",
            [SceneName.Lobby] = "Lobby",
            [SceneName.CutScene] = "LDH_CutScene",
            [SceneName.InGame] = "PMS_InGame",
        };

        public static string GetSceneName(SceneName sceneName)
        {
            return sceneNameDict.TryGetValue(sceneName, out string value) ? value : null;
        }
        
        
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
            EnterLobby,
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
            JoinRoomMaxPlayerError,
            JoinRoomStatusError,
            
        }
        
        public enum RoomStatus
        {
            Waiting,
            Playing
        }
        
        
        public enum SceneName
        {
            Title,
            Lobby,
            CutScene,
            InGame
        }
    }
}