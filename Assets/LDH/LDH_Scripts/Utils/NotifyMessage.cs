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
            
            [NotifyMessageType.RoomCodeSuccess] = new MessageEntity(
                "Room Joined", 
                "Successfully joined the room using the code.", NotifyType.Check
            ),

            [NotifyMessageType.RoomCodeError] = new MessageEntity(
                "Invalid Code", 
                "No room was found with the entered code. Please check and try again.", NotifyType.Error
            ),

            [NotifyMessageType.RoomCodeEmpty] = new MessageEntity(
                "Empty Code", 
                "Please enter a room code before proceeding.", NotifyType.Error
            ),
        };

    }
}