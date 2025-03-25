using DentalApp.Models;
using DentalApp.Models.Enum;
using System.Globalization;

namespace DentalApp.Models
{
    public partial class User : BaseModel
    {
        public int RoleId { get; set; }
        public string? Username { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
        public UserStatus Status { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string? MiddleName { get; set; }
        public string LastName { get; set; } = String.Empty;

        public string FullName
        {
            get { return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(LastName + ", " + FirstName); }
        }

        public string? Mobile { get; set; }
        public DateTime? BirthDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public string? Email { get; set; }
        public string? Occupation { get; set; }

        public string? Address { get; set; }
        public string? Note { get; set; }
        public DateTime? LastLogin { get; set; }
        public virtual Role? Role { get; set; }
        public virtual ICollection<Dentist> Dentists { get; set; } = new HashSet<Dentist>();
        public virtual ICollection<Patient> Patients { get; set; } = new HashSet<Patient>();
    }
}
