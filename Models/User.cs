using SQLite;

namespace DentalApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

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

        [Ignore]
        public string RoleName { get; set; } // Maintains compatibility with your usage.


        [Ignore]
        public int Status { get; set; } // Maintains compatibility with your current structure.

        // Computed property for FullName
        [Ignore]
        public string FullName
        {
            get
            {
                // Combine FirstName, MiddleName, and LastName if MiddleName exists
                return string.IsNullOrWhiteSpace(MiddleName)
                    ? $"{FirstName} {LastName}"
                    : $"{FirstName} {MiddleName} {LastName}";
            }
        }
    }
}
