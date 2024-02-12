using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateChat.Api.DomainModels
{
    public class Chat
    {
        public int Chat_Id { get; set; }
        public string Chat_UserId { get; set; }
        public string Name { get; set; }

    }
}
