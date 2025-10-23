using SocNetwork.Models.Db;

namespace SocNetwork.Models.ViewModel
{
    public class ChatViewModel
    {
        public UserViewModel Friend { get; set; }
        public IEnumerable<Message> Messages { get; set; }
        public string CurrentUserId { get; set; }
    }
}
