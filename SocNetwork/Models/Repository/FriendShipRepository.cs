using Microsoft.EntityFrameworkCore;
using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public class FriendShipRepository : Repository<FriendShip>, IFriendShipRepository
    {
        public FriendShipRepository(ApplicationDbContext db) : base(db)
        {
        }

        public async Task<IEnumerable<FriendShip>> GetFriendsAsync(string userId)
        {
            return await _dbSet
                .Where(f => f.IsAccepted && (f.RequesterId == userId || f.AddresseeId == userId))
                .ToListAsync();
        }

        public async Task<FriendShip> GetFriendshipAsync(string requesterId, string addresseeId)
        {
            return await _dbSet.FirstOrDefaultAsync(f =>
             f.RequesterId == requesterId && f.AddresseeId == addresseeId ||
             f.RequesterId == addresseeId && f.AddresseeId == requesterId);
        }

        public async Task<IEnumerable<FriendShip>> GetPendingRequestAsync(string userId)
        {
           return await _dbSet
                .Where(f=>!f.IsAccepted && f.AddresseeId == userId)
                .ToListAsync();
        }
    }
}
