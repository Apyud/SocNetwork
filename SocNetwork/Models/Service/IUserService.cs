using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public interface IUserService
    {
        Task<UserViewModel> GetUserProfileASync(string userId);
        Task UpdateUserProfileAsync(UserEditViewModel model);
        
        Task<IEnumerable<UserViewModel>> SearchUsersAsync(string query, string currentUserId);

    }
}
