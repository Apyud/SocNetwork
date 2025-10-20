using AutoMapper;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public class UserService : IUserService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        public UserService(IUnitOfWork unitOfWork, IMapper imapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = imapper;
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
