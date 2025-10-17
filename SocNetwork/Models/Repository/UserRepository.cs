using Microsoft.EntityFrameworkCore;
using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByIdAsync(string id)
        {
            return await _dbSet.FindAsync(id);
        }

        public async Task<User> GetByUserNameAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName == userName);
        }

        public async Task<User> SearchUserAsync(string userName)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.UserName.Contains(userName));
        }

        public async Task<IEnumerable<User>> SearchUsersAsync(string searchTerm)
        {
            return await _dbSet
                 .Where(u => u.FirstName.Contains(searchTerm) ||
                 u.LastName.Contains(searchTerm) ||
                 u.Email.Contains(searchTerm))
                 .ToListAsync();
        }

        public async Task UpdateUserProfileASync(User user)
        {
            // Просто используем базовый метод Update из Repository<T>
            base.Update(user); // или просто Update(user);
        }


    }
}
