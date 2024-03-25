using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using PrivateChat.Api.DataModels;
using PrivateChat.Api.DBContext;
using PrivateChat.Api.Encryption;
using PrivateChat.Api.SignalR;
using System.IdentityModel.Tokens.Jwt;
using DomainModel = PrivateChat.Api.DomainModels;
using efUser = PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.Repositories
{
    public class SqlChatRepository : IChatRepository
    {
        private readonly AdminContext context;
        private readonly IHubContext<SignalRHub> hub;
        private readonly ISymmetricEncryption symmetricEncryption;

        public SqlChatRepository(AdminContext context, IHubContext<SignalRHub> hub, ISymmetricEncryption symmetricEncryption)
        {
            this.context = context;
            this.hub = hub;
            this.symmetricEncryption = symmetricEncryption;
        }

        public async Task<efUser.User> AddUserChat(DomainModel.UserChat data)
        {
            var user = await this.context.UserEn.FirstOrDefaultAsync(res => res.Id.Equals(data.User_Id));
            if (user != null)
            {
                Chat chat = new Chat()
                {
                    Chat_UserId = data.Chat_Id,
                    Name = data.ChatUserName
                };
                if (user.Chats == null || user.Chats.Count <= 0)
                {
                    user.Chats = new List<Chat>();
                }
                user.Chats.Add(chat);
            }
            this.context.UserEn.Update(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task<List<Message>> GetUserClientChatMessage(string userId, string chatId)
        {
            var messages = await context.Message.AsQueryable().
               Where(res => (res.From_UserId.Equals(userId) && res.To_ChatId.Equals(chatId)) || (res.To_ChatId.Equals(userId) && res.From_UserId.Equals(chatId))).
               OrderBy(s => s.SendAt).ToListAsync();

            messages.ForEach(s =>
            {
                s.MessageText = symmetricEncryption.DecryptMessage(s.MessageText);
            });

            return messages;
        }
        
       

        public async Task<List<Message>> GetUserQueries()
        {
            var messages = await context.Message.OrderBy(s => s.SendAt).ToListAsync();
            return messages;
        }

        public async Task SendMessage(Message message)
        {
            var msg = JsonConvert.SerializeObject(message);
            var newMessage = JsonConvert.DeserializeObject<Message>(msg);

            message.MessageText = symmetricEncryption.EncryptMessage(message.MessageText);
            await context.Message.AddAsync(message);
            await context.SaveChangesAsync();
              
            await hub.Clients.All.SendAsync("message", newMessage);
        }
        public async Task SendUserTextQuery(Message message)
        {
            var msg = JsonConvert.SerializeObject(message);
            var newMessage = JsonConvert.DeserializeObject<Message>(msg);

            //message.MessageText = symmetricEncryption.EncryptMessage(message.MessageText);
            await context.Message.AddAsync(message);
            await context.SaveChangesAsync();

            //await hub.Clients.All.SendAsync("message", newMessage);
        }

        public async Task AddMentorComment(Message message, bool isMentor)
        {
            var msg = JsonConvert.SerializeObject(message);
            var newMessage = JsonConvert.DeserializeObject<Message>(msg);

            var existingMessage = await this.context.Message.Include(x => x.Comments).FirstOrDefaultAsync(res => res.Message_Id.Equals(message.Message_Id));
            if (existingMessage != null)
            {
                Comments comment = new Comments()
                {
                    text = message.MessageText,
                    IsAdmin = isMentor
                };
                if (existingMessage.Comments == null || existingMessage.Comments.Count <= 0)
                {
                    existingMessage.Comments = new List<Comments>();
                }
                existingMessage.Comments.Add(comment);
                this.context.Message.Update(existingMessage);
                await context.SaveChangesAsync();
            }
            //await hub.Clients.All.SendAsync("message", newMessage);
        }

        public async Task<ICollection<Comments>> GetIssueChatMessage(int messageId)
        {
            var existingMessage = await this.context.Message.Include(x => x.Comments).FirstOrDefaultAsync(res => res.Message_Id.Equals(messageId));
            if(existingMessage != null)
            {
                var comments = existingMessage.Comments;
                return comments;
            }

            return null;
            
        }
    }
}
