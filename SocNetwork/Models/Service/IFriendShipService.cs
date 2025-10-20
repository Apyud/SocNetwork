using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public interface IFriendShipService
    {
        Task SendFriendrequestAsync(string userId, string friendId);
        Task AcceptFriendRequestAsync(int requestId);
        Task DeclineFriendRequestAsync(int requestId);
        Task<IEnumerable<UserViewModel>> GetFriendsAsync(string  userId);
        Task<IEnumerable<UserViewModel>> GetPendingRequestsAsync(string userId);
    }
}
