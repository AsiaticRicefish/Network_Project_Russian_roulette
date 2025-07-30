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
            [NotifyMessageType.NickNameSuccess] = new ("Nickname Set", "You're now ready to join the lobby.", NotifyType.Check),
            [NotifyMessageType.NickNameError] = new ("Invalid Nickname", "Please enter a valid nickname.", NotifyType.Error),
            
        };

    }
}