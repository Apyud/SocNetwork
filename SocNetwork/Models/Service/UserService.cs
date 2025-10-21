using AutoMapper;
using Microsoft.Extensions.Logging;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<UserService> _logger;
        public UserService(IUnitOfWork unitOfWork, IMapper imapper, ILogger<UserService> logger)
        {
            _unitOfWork = unitOfWork;
            _mapper = imapper;
            _logger = logger;
        }

        public async Task<UserViewModel> GetUserProfileASync(string userId)
        {
            var userRepository = _unitOfWork.GetRepository<User>() as UserRepository;

            if (userRepository == null)
                throw new InvalidOperationException("User repository not found");

            var user = await userRepository.GetByIdAsync(userId);

            if (user == null)
                throw new ArgumentException("User not found");

            return _mapper.Map<UserViewModel>(user);
        }

        public async Task<IEnumerable<UserViewModel>> SearchUsersAsync(string query, string currentUserId)
        {
            // Получаем репозитории
            var userRepo = _unitOfWork.GetRepository<User>();
            var friendShipRepo = _unitOfWork.GetRepository<FriendShip>();

            // Загружаем данные
            var allUsers = await userRepo.GetAllAsync();
            var allFriendships = await friendShipRepo.GetAllAsync();

            // Ищем пользователей по имени, фамилии, логину или email
            var foundUsers = allUsers
                .Where(u =>
                    u.Id != currentUserId && // исключаем самого себя
                    (
                        (!string.IsNullOrEmpty(u.FirstName) && u.FirstName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.LastName) && u.LastName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.UserName) && u.UserName.Contains(query, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(u.Email) && u.Email.Contains(query, StringComparison.OrdinalIgnoreCase))
                    ))
                .ToList();

            // Получаем дружбы, где участвует текущий пользователь
            var userFriendships = allFriendships
                .Where(f => f.RequesterId == currentUserId || f.AddresseeId == currentUserId)
                .ToList();

            // Маппим пользователей
            var result = _mapper.Map<List<UserViewModel>>(foundUsers);

            // Добавляем статус дружбы
            foreach (var user in result)
            {
                var friendship = userFriendships.FirstOrDefault(f =>
                    (f.RequesterId == currentUserId && f.AddresseeId == user.Id) ||
                    (f.AddresseeId == currentUserId && f.RequesterId == user.Id));

                if (friendship != null)
                {
                    if (friendship.IsAccepted)
                    {
                        user.IsFriend = true;
                    }
                    else if (friendship.RequesterId == currentUserId)
                    {
                        user.IsPendingRequestSent = true;
                    }
                    else if (friendship.AddresseeId == currentUserId)
                    {
                        user.IsPendingRequestReceived = true;
                    }
                }
            }

            return result;
        }




        public async Task UpdateUserProfileAsync(UserEditViewModel model)
        {
            var userRepository = _unitOfWork.GetRepository<User>() as UserRepository;
            var user = await userRepository.GetByIdAsync(model.Id);
            if (user == null)
                throw new ArgumentException("User not found");

            if (model.BirthDate.Offset != TimeSpan.Zero)
            {
                model.BirthDate = model.BirthDate.ToUniversalTime();
            }
            _mapper.Map(model, user);
            await userRepository.UpdateUserProfileASync(user);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
