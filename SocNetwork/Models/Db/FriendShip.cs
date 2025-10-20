namespace SocNetwork.Models.Db
{
    public class FriendShip
    {
        public int Id { get; set; }

        public string RequesterId { get; set; }  // кто отправил заявку
        public string AddresseeId { get; set; }  // кто получил заявку

        public bool IsAccepted { get; set; }     // true — друзья, false — заявка
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public User Requester { get; set; }
        public User Addressee { get; set; }
    }
}
