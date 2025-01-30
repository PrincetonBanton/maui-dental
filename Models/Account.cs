using SQLite;

namespace DentalApp.Models
{
    public class Account
    {
        [PrimaryKey, AutoIncrement]
        public int AccountId { get; set; }

        public string Username { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public DateTime BirthDate { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public string Note { get; set; }

        public int RoleId { get; set; }

        [Ignore]
        public Role Role { get; set; }
    }
}
