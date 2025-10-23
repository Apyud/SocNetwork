namespace SocNetwork.Models.Db
{
    public class Message
    {
        public int Id { get; set; }
        public string Text { get; set; }
        public string SenderId { get; set; } // Отправитель
        public string ReceiverId {  get; set; } // Получатель
        public DateTime SentAt { get; set; } // Время отправки 
        public bool IsRead { get; set; } = false; // Прочитано ли сообщение


        public User Sender { get; set; } // Отправитель для навигации 

        public User Receiver { get; set; } // Получатель для навигации 
    }
}
