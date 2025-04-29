using System;


namespace DentalApp.Models
{
    public class Appointment : BaseModel
    {
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DentistId { get; set; }
        public string? DentistName { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? Date { get; set; }

        public virtual User? User { get; set; }
        public string FormattedDisplay => $"{StartDate:MM/dd/yyyy} - {Description}";

    }
}
