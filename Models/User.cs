using SQLite;

namespace DentalApp.Models
{
    public class User
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public int RoleId { get; set; }

        public string? Username { get; set; } 
        public string? Password { get; set; }  
        public byte[]? PasswordHash { get; set; }  
        public byte[]? PasswordSalt { get; set; } 
        public string? FirstName { get; set; } 
        public string? MiddleName { get; set; } 
        public string? LastName { get; set; } 

        public DateTime BirthDate { get; set; } 
        public string? Mobile { get; set; }  
        public string? Email { get; set; } 
        public string? Address { get; set; }  
        public string? Note { get; set; }  
        public string? Occupation { get; set; }  

        public DateTime? CreatedOn { get; set; } 
        public DateTime? LastLogin { get; set; } 

        [Ignore]
        public string RoleName { get; set; }

        [Ignore]
        public int Status { get; set; }

        [Ignore]
        public string FullName =>
            string.IsNullOrWhiteSpace(MiddleName)
                ? $"{FirstName} {LastName}"
                : $"{FirstName} {MiddleName} {LastName}";
        [Ignore]
        public string Initials =>
            (!string.IsNullOrEmpty(FirstName) && !string.IsNullOrEmpty(LastName))
                ? $"{FirstName[0]}{LastName[0]}".ToUpper()
                : "??";
    }
}
