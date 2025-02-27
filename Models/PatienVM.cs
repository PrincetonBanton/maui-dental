using System;

namespace DentalApp.Models
{
    // Enum to represent user status
    public enum UserStatus
    {
        Active = 1,
        Inactive = 2,
        Suspended = 3
    }

    // ViewModel for Patient
    public class PatientVM
    {
        public int Id { get; set; }
        public string? FirstName { get; set; }
        public string? MiddleName { get; set; }
        public string? LastName { get; set; }
        public string? FullName => $"{LastName}, {FirstName}";
        public DateTime BirthDate { get; set; }
        public string? Mobile { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? Note { get; set; }
        public string? PatientNo { get; set; }
        public DateTime? LastLogin { get; set; }
        public UserStatus Status { get; set; }
    }
}
