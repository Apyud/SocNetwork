using SocNetwork.Models.Db;

namespace SocNetwork.Models.ViewModel
{
    public class UserViewModel
    {
       // public User User {  get; set; }
       //public UserViewModel(User user) 
       //{
       //    User = user;
       //}
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        public int PostsCount { get; set; }
        public string? Image { get; set; }

        public DateTimeOffset BirthDate { get; set; }
        public string? Status { get; set; }

        public string? About { get; set; }
    }
}
