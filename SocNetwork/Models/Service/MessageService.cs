using Microsoft.EntityFrameworkCore;
using SocNetwork.Models.Db;
using SocNetwork.Models.Repository;

namespace SocNetwork.Models.Service
{
    public class MessageService : IMessageService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IFriendShipService _friendShipService;

        public MessageService(IUnitOfWork unitOfWork, IFriendShipService friendShipService)
        {
            _unitOfWork = unitOfWork;
            _friendShipService = friendShipService;
        }

        public async Task<IEnumerable<Message>> GetConversationAsync(string userId, string friendId)
        {
           if(string.IsNullOrWhiteSpace(userId) ||string.IsNullOrWhiteSpace(friendId))
            {
                throw new ArgumentException("Некорректные ID пользователей");
            }

           var repo = _unitOfWork.GetRepository<Message>();

           var message = await repo.Query()
                .Where(m => (m.SenderId == userId && m.ReceiverId == friendId) || (m.SenderId == friendId && m.ReceiverId == userId))
                .OrderBy(m => m.SentAt)
                .ToListAsync();

            return message;      
        }

        public async Task MarkAsReadAsync(string messageId)
        {
            var repo = _unitOfWork.GetRepository<Message>();
            var message = await repo.GetByIdAsync(messageId);
            if (message == null)
            {
                throw new InvalidOperationException("Сообщение не найдено");
            }
           
            message.IsRead = true;
            await _unitOfWork.SaveChangesAsync();
        }

        public async Task SendMessageAsync(string senderId, string receiverId, string text)
        {
           if (string.IsNullOrEmpty(text))
            {
                throw new ArgumentException("Текст не может быть пустым");
            }

            var message = new Message()
            {
                SenderId = senderId,
                Text = text,
                ReceiverId = receiverId,
                SentAt = DateTime.UtcNow,
                IsRead = false,
            };

            var repo = _unitOfWork.GetRepository<Message>();
            await repo.CreateAsync(message);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}
