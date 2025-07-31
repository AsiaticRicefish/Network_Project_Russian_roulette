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
            [NotifyMessageType.SignupSuccess] = new ("Sign Up Successful", "Your account has been created. Welcome!", NotifyType.Check),
            [NotifyMessageType.SignupError] = new ("Sign Up Failed", "Something went wrong. Please try again.", NotifyType.Error),
            
            [NotifyMessageType.EmailCheckSuccess] = new ("Valid Email", "Your email is valid.", NotifyType.Check),
            [NotifyMessageType.EmailCheckError] = new ("Invalid Email", "Your email is invalid.", NotifyType.Error),

            
            [NotifyMessageType.LoginSuccess] = new ("Nickname Set", "You're now ready to join the lobby.", NotifyType.Check),
            [NotifyMessageType.LoginError] = new ("Login Failed", "Invalid email or password.", NotifyType.Error),
            
            
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