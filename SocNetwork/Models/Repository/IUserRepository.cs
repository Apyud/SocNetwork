using Microsoft.EntityFrameworkCore.Update.Internal;
using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByIdAsync (string  id);
        Task<User> GetByUserNameAsync (string userName);
        Task<User> GetByEmailAsync (string email);
        Task<User> SearchUserAsync(string userName);
        Task<IEnumerable<User>> SearchUsersAsync(string searchTerm);
        Task UpdateUserProfileASync(User user);
    }

}
