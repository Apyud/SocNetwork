using System.Collections.Generic;

namespace SocNetwork.Models.ViewModel
{
    public class FriendListViewModel
    {
        /// <summary>Подтверждённые друзья текущего пользователя</summary>
        public List<UserViewModel> Friends { get; set; } = new();

        /// <summary>Входящие заявки (другие пользователи отправили мне)</summary>
        public List<UserViewModel> IncomingRequests { get; set; } = new();

        /// <summary>Исходящие заявки (я отправил другим пользователям)</summary>
        public List<UserViewModel> SentRequests { get; set; } = new();
    }
}
