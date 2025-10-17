using Microsoft.AspNetCore.Identity;

namespace SocNetwork.Models.Db
{
    public class User:IdentityUser
    {
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string? MiddleName { get; set; }

        public DateTimeOffset BirthDate { get; set; }
        public string? Image { get; set; }

        public string? Status { get; set; }

        public string? About { get; set; }

        public string GetFullName()
        {
            return FirstName + " " + MiddleName + " " + LastName;
        }

    }
}
