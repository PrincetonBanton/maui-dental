﻿using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class PatientVM : BaseModel
    {
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
