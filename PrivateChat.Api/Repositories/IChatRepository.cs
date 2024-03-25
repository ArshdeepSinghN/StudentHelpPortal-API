using PrivateChat.Api.DataModels;
using DomainModel = PrivateChat.Api.DomainModels;
using efUser = PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.Repositories
{
    public interface IChatRepository
    {
        Task<efUser.User> AddUserChat(DomainModel.UserChat data);

        Task<List<Message>> GetUserClientChatMessage(string userId, string chatId);
        Task<List<Message>> GetUserQueries();

        Task SendMessage(Message message);
        Task SendUserTextQuery(Message message);
        Task AddMentorComment(Message message, bool IsMentor);
        Task<ICollection<Comments>> GetIssueChatMessage(int messageId);


    }
}
