using System.Collections.Generic;
using static Utils.Define_LDH;
namespace Utils
{
    
    public class MessageEntity
    {
        public string Title;
        public string Description;
        public NotifyType NotifyType;
            
            
        public MessageEntity(string title, string description, NotifyType notifyType)
        {
            Title = title;
            Description = description;
            NotifyType = notifyType;
        }
    }
    
    public class NotifyMessage
    {
       

        public static readonly Dictionary<NotifyMessageType, MessageEntity> MessageEntities = new Dictionary<NotifyMessageType, MessageEntity>
        {
            [NotifyMessageType.NicknameSuccess] = new ("Nickname Set", "You're now ready to join the lobby.", NotifyType.Check),
            [NotifyMessageType.NicknameError] = new ("Invalid Nickname", "Please enter a valid nickname.", NotifyType.Error),
            
            
            [NotifyMessageType.CreeateRoomSuccess] = new ("Room Created", "Your room has been successfully created.", NotifyType.Check),
            [NotifyMessageType.CreateRoomError] = new ("Failed to Create Room", "An error occurred while creating the room. Please try again.", NotifyType.Error),
            
            [NotifyMessageType.RoomCodeSuccess] = new ("Room Created", "Your room has been successfully created.", NotifyType.Check),
            [NotifyMessageType.RoomCodeError] = new ("Failed to Create Room", "An error occurred while creating the room. Please try again.", NotifyType.Error),
        };

    }
}