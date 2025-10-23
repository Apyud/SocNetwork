using SocNetwork.Models.Db;

namespace SocNetwork.Models.Service
{
    public interface IMessageService
    {
        Task SendMessageAsync(string senderId, string receiverId, string text);
        Task<IEnumerable<Message>> GetConversationAsync(string userId, string friendId);
        Task MarkAsReadAsync(string messageId);

    }
}
