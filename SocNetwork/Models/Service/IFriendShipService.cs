using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public interface IFriendShipService
    {
        Task SendFriendRequestAsync(string requesterId, string addresseeId);

        // Принимаем/отклоняем по паре userId'ов:
        // requesterId — кто отправил заявку
        // currentUserId — текущий пользователь (кому пришла заявка)
        Task AcceptFriendRequestAsync(string requesterId, string currentUserId);
        Task DeclineFriendRequestAsync(string requesterId, string currentUserId);

        Task<IEnumerable<UserViewModel>> GetFriendsAsync(string userId);
        Task<IEnumerable<UserViewModel>> GetPendingRequestsAsync(string userId);

        Task UnfriendAsync(string userId, string friendId);
    }
}
