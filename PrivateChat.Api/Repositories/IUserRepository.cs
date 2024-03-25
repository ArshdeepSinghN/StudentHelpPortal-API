using PrivateChat.Api.DataModels;
using System.Threading.Tasks;
using efUser = PrivateChat.Api.Entity.Models;

namespace PrivateChat.Api.Repositories
{
    public interface IUserRepository
    {
        List<efUser.User> GetAllUsers();
        Task<efUser.User> GetUserById(string id);

        void SaveUserData(User Data);
    }
}
