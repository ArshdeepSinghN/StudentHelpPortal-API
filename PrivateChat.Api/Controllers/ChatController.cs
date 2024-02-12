using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using PrivateChat.Api.DataModels;
using DomainModel = PrivateChat.Api.DomainModels;
using PrivateChat.Api.Repositories;
using PrivateChat.Api.DomainModels;

namespace PrivateChat.Api.Controllers
{
    public class ChatController : Controller
    {
        private readonly IChatRepository chatRepository;
        private readonly IUserRepository userRepository;
        private readonly IMapper mapper;

        public ChatController(IChatRepository chatRepository, IUserRepository userRepository, IMapper mapper)
        {
            this.chatRepository = chatRepository;
            this.userRepository = userRepository;
            this.mapper = mapper;
        }


        [HttpPost]
        [Route("[controller]")]
        public async Task<IActionResult> AddUserChatClient([FromBody] DomainModel.UserChat data)
        {
            var user = await this.chatRepository.AddUserChat(data);
            // we need to add one more entry in user table for client 
            // either we should first send notification to client that user wants to add u once it is yes add enteries in user mapping chat for both clients
            if (user != null)
            {
                var clients = mapper.Map<UserChatClients>(user);
                return Ok(clients);
            }


            return Ok(null);
        }

        [HttpGet]
        [Route("[controller]/{id}")]
        public async Task<IActionResult> GetUserChatClientsByUserId(string id)
        {
            var user = await userRepository.GetUserById(id);
            if (user != null)
            {
                var clients = mapper.Map<UserChatClients>(user);
                return Ok(clients);
            }
            return Ok(null);
        }

        [HttpGet]
        [Route("[controller]/GetChatMessage/{userId}/{chatId}")]
        public async Task<IActionResult> GetUserChatClientMessage(string userId, string chatId)
        {
            var result = await chatRepository.GetUserClientChatMessage(userId, chatId);
            if (result != null && result.Count>0)
            {
                var messages = mapper.Map<List<DomainModel.Message>>(result);
                return Ok(messages);
            }


            return Ok(null);
        }

        [HttpPost]
        [Route("[controller]/AddChatMessage")]
        public async Task<IActionResult> AddMessage([FromBody] DomainModel.Message message)
        {
            // encyrpt message and then post it

            var clientMessage = mapper.Map<DataModels.Message>(message);

            await chatRepository.SendMessage(clientMessage);
            mapper.Map<DomainModel.Message>(clientMessage);
            return Ok(message);
        }

        [HttpPost]
        [Route("[controller]/SendUserTextQuery")]
        public async Task<IActionResult> SendUserTextQuery([FromBody] DomainModel.Message message)
        {
            // encyrpt message and then post it

            var clientMessage = mapper.Map<DataModels.Message>(message);

            await chatRepository.SendUserTextQuery(clientMessage);
            mapper.Map<DomainModel.Message>(clientMessage);
            return Ok(message);
        }

        [HttpGet]
        [Route("[controller]/GetUserQueries")]
        public async Task<IActionResult> GetUserQueries()
        {
            
           var result = await chatRepository.GetUserQueries();
            if (result != null && result.Count > 0)
            {
                var messages = mapper.Map<List<DomainModel.Message>>(result);
                return Ok(messages);
            }


            return Ok(null);
        }

        [HttpPost]
        [Route("[controller]/AddMentorComment")]
        public async Task<IActionResult> AddMentorComment([FromBody] DomainModel.UserMessage message)
        {
            // encyrpt message and then post it

            var clientMessage = mapper.Map<DataModels.Message>(message.Message);

            await chatRepository.AddMentorComment(clientMessage, message.IsMentor);
            mapper.Map<DomainModel.Message>(clientMessage);
            return Ok(message);
        }

        [HttpGet]
        [Route("[controller]/GetIssueChatMessage/{messageId}")]
        public async Task<IActionResult> GetIssueChatMessage(int messageId)
        {
            var result = await chatRepository.GetIssueChatMessage(messageId);
            return Ok(result);

        }

    }
}
