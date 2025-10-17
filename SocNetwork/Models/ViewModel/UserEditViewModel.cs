namespace SocNetwork.Models.ViewModel
{
    public class UserEditViewModel
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string MiddleName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset JoinDate { get; set; }
        public int PostsCount { get; set; }
        public string? Image { get; set; }

        public DateTimeOffset BirthDate { get; set; }
        public string? Status { get; set; }

        public string? About { get; set; }
    }
}
