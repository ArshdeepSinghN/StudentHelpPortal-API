using Microsoft.EntityFrameworkCore;
using PrivateChat.Api.DataModels;
using PrivateChat.Api.DBContext;
using efUser= PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.Repositories
{
    public class SqlUserRepository : IUserRepository
    {
        private readonly AdminContext context;

        public SqlUserRepository(AdminContext context)
        {
            this.context = context;
        }

        public List<efUser.User> GetAllUsers()
        {
            return context.UserEn.ToList();
        }

        public async Task<efUser.User> GetUserById(string id)
        {
            var user = context.UserEn.Where(res => res.Id == id).Include(c=>c.Chats).FirstOrDefault();
            return user;
        }

        public void SaveUserData(User Data)
        {
            context.User.Add(Data);
            context.SaveChanges();
        }
    }
}
