using AutoMapper;
using Microsoft.Extensions.Configuration.UserSecrets;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public class FriendShipService : IFriendShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFriendShipService _friendShipService;
        

        public FriendShipService(IUnitOfWork unitOfWork, IMapper mapper, IFriendShipService friendShipService)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _friendShipService = friendShipService;
        }

        public async Task AcceptFriendRequestAsync(int requestId)
        {
            var friendshipRepo = _unitOfWork.GetRepository<FriendShip>();
            var existing = await friendshipRepo.GetByIdAsync(requestId);

            if (existing == null)
            {
                throw new InvalidOperationException("Заявка не найдена");
            }
            if (existing.IsAccepted)
            {
                throw new InvalidOperationException("Эта заявка уже принята");
            }
            existing.IsAccepted = true;
            //await _friendShipService.AcceptFriendRequestAsync(requestId);
            await _unitOfWork.SaveChangesAsync();
        }

        public Task DeclineFriendRequestAsync(int requestId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserViewModel>> GetFriendsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<UserViewModel>> GetPendingRequestsAsync(string userId)
        {
            throw new NotImplementedException();
        }

        public async Task SendFriendrequestAsync(string requesterId, string addresseeId)
        {
           var friendshipRepo = _unitOfWork.GetRepository<FriendShip>(); 
            if (requesterId == addresseeId)
                throw new InvalidOperationException("Нельзя добавить в друзья самого себя");

            var existing = await friendshipRepo
                .FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                (f.RequesterId == addresseeId && f.AddresseeId == requesterId));
            var friendShip = new FriendShip
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                IsAccepted = false,
                CreatedAt = DateTime.UtcNow
            };
            await friendshipRepo.CreateAsync(friendShip);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
