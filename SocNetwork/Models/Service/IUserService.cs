using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public interface IUserService
    {
        Task<UserViewModel> GetuserProfileASync(string userId);
        Task UpdateUserProfileAsync(UserEditViewModel model);

    }
}
