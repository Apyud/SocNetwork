using SocNetwork.Models.Db;

namespace SocNetwork.Models.Repository
{
    public interface IFriendShipRepository : IRepository<FriendShip>
    {
        Task<FriendShip> GetFriendshipAsync(string requestId, string addresseeId);
        Task<IEnumerable<FriendShip>> GetFriendsAsync(string userId); // получить друга
        Task<IEnumerable<FriendShip>> GetPendingRequestAsync(string userId); // получить ожидающий запрос 
    }
}
