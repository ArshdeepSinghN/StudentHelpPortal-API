using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PrivateChat.Api.DataModels
{
    public class Chat
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        // will be userId   
        public int Chat_Id { get; set; }
        public string Chat_UserId { get; set; }
        public string Name { get; set; }

        //public virtual ICollection<User> Users { get; set; }   
    }
}
