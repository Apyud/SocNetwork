using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;
using SocNetwork.Models.ViewModel;

namespace SocNetwork.Models.Service
{
    public class FriendShipService : IFriendShipService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FriendShipService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        //  Принять заявку
        public async Task AcceptFriendRequestAsync(string requesterId, string currentUserId)
        {
            var repo = _unitOfWork.GetRepository<FriendShip>();
            var friendships = await repo.GetAllAsync();

            var friendship = friendships.FirstOrDefault(f =>
                !f.IsAccepted &&
                ((f.RequesterId == requesterId && f.AddresseeId == currentUserId) ||
                 (f.RequesterId == currentUserId && f.AddresseeId == requesterId)));

            if (friendship == null)
                throw new InvalidOperationException("Заявка не найдена.");

            friendship.IsAccepted = true;
            await _unitOfWork.SaveChangesAsync();
        }

        // Отклонить заявку
        public async Task DeclineFriendRequestAsync(string requesterId, string currentUserId)
        {
            var repo = _unitOfWork.GetRepository<FriendShip>();
            var friendships = await repo.GetAllAsync();

            var friendship = friendships.FirstOrDefault(f =>
                !f.IsAccepted &&
                ((f.RequesterId == requesterId && f.AddresseeId == currentUserId) ||
                 (f.RequesterId == currentUserId && f.AddresseeId == requesterId)));

            if (friendship == null)
                throw new InvalidOperationException("Заявка не найдена.");

            repo.Delete(friendship);
            await _unitOfWork.SaveChangesAsync();
        }

        // Получить список друзей
        public async Task<IEnumerable<UserViewModel>> GetFriendsAsync(string userId)
        {
            var friendShipRepo = _unitOfWork.GetRepository<FriendShip>();

            // Загружаем дружбы, где участвует текущий пользователь, и сразу подгружаем пользователей
            var friendships = await friendShipRepo.Query()
                .Where(f => f.IsAccepted && (f.RequesterId == userId || f.AddresseeId == userId))
                .Include(f => f.Requester)
                .Include(f => f.Addressee)
                .ToListAsync();

            // Определяем, кто является другом (в зависимости от того, кто из них текущий пользователь)
            var friends = friendships
                .Select(f => f.RequesterId == userId ? f.Addressee : f.Requester)
                .Distinct()
                .ToList();

            // Преобразуем в UserViewModel
            return _mapper.Map<IEnumerable<UserViewModel>>(friends);
        }


        // Получить все ожидающие заявки
        public async Task<IEnumerable<UserViewModel>> GetPendingRequestsAsync(string userId)
        {
            var friendShipRepo = _unitOfWork.GetRepository<FriendShip>();
            var userRepo = _unitOfWork.GetRepository<User>();

            var pendingFriendships = (await friendShipRepo.GetAllAsync())
                .Where(f => !f.IsAccepted && (f.RequesterId == userId || f.AddresseeId == userId))
                .ToList();

            if (!pendingFriendships.Any())
                return Enumerable.Empty<UserViewModel>();

            var relatedUserIds = pendingFriendships
                .Select(f => f.RequesterId == userId ? f.AddresseeId : f.RequesterId)
                .Distinct()
                .ToList();

            var allUsers = await userRepo.GetAllAsync();
            var relatedUsers = allUsers.Where(u => relatedUserIds.Contains(u.Id)).ToList();

            var pendingUsers = _mapper.Map<List<UserViewModel>>(relatedUsers);

            foreach (var pendingUser in pendingUsers)
            {
                var friendship = pendingFriendships.First(f =>
                    (f.RequesterId == userId && f.AddresseeId == pendingUser.Id) ||
                    (f.AddresseeId == userId && f.RequesterId == pendingUser.Id));

                if (friendship.AddresseeId == userId)
                    pendingUser.IsPendingRequestReceived = true;
                else
                    pendingUser.IsPendingRequestSent = true;
            }

            return pendingUsers
                .OrderByDescending(u => u.IsPendingRequestReceived)
                .ThenBy(u => u.FullName)
                .ToList();
        }

        // Отправка заявки
        public async Task SendFriendRequestAsync(string requesterId, string addresseeId)
        {
            if (requesterId == addresseeId)
                throw new InvalidOperationException("Нельзя добавить в друзья самого себя.");

            var repo = _unitOfWork.GetRepository<FriendShip>();

            var existing = await repo.FirstOrDefaultAsync(f =>
                (f.RequesterId == requesterId && f.AddresseeId == addresseeId) ||
                (f.RequesterId == addresseeId && f.AddresseeId == requesterId));

            if (existing != null)
            {
                if (existing.IsAccepted)
                    throw new InvalidOperationException("Вы уже друзья.");
                else
                    throw new InvalidOperationException("Заявка уже отправлена и ожидает подтверждения.");
            }

            var friendship = new FriendShip
            {
                RequesterId = requesterId,
                AddresseeId = addresseeId,
                IsAccepted = false,
                CreatedAt = DateTime.UtcNow
            };

            await repo.CreateAsync(friendship);
            await _unitOfWork.SaveChangesAsync();
        }

        //  Удалить друга
        public async Task UnfriendAsync(string userId, string friendId)
        {
            var repo = _unitOfWork.GetRepository<FriendShip>();
            var friendship = await repo.FirstOrDefaultAsync(f =>
                f.IsAccepted &&
                ((f.RequesterId == userId && f.AddresseeId == friendId) ||
                 (f.RequesterId == friendId && f.AddresseeId == userId)));

            if (friendship == null)
                throw new InvalidOperationException("Дружба не найдена.");

            repo.Delete(friendship.Id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
