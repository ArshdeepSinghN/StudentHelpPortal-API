
using PrivateChat.Api.DataModels;

namespace PrivateChat.Api.DomainModels
{
    public class Message
    {
        public int? Message_Id { get; set; }
        public string From_UserId { get; set; }
        public string From_UserName { get; set; }
        public string To_ChatId { get; set; }
        public string MessageTitle { get; set; }
        public string MessageText { get; set; }
        public  ICollection<Comments> Comments { get; set; }
        public int? IsMessageSeen { get; set; }
        public DateTime? SendAt { get; set; }
    }

    public class UserMessage
    {
        public Message Message { get; set; }
        public bool IsMentor { get; set; }

    }
}
