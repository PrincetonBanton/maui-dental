using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class UserVM :  BaseModel
    {
        public string? Username { get; set; }
        public string? Password { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{LastName}, {FirstName}";
        public DateTime BirthDate { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public int? RoleId { get; set; }
        public string? RoleName { get; set; }
        public DateTime? LastLogin { get; set; }
        public UserStatus Status { get; set; }
    }
}
