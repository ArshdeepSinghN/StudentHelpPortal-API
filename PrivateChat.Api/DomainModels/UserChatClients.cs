using DataModels = PrivateChat.Api.DataModels;
namespace PrivateChat.Api.DomainModels
{
    public class UserChatClients
    {
        public ICollection<Chat> Chats { get; set; }
    }
}
