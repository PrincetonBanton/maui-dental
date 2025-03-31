using System;

namespace DentalApp.Models
{
    public class SaleVM : BaseModel
    {
        public DateTime SaleDate { get; set; }
        public int SaleId { get; set; }
        public string? PatientName { get; set; }
        public string? DentistName { get; set; }
        public decimal Total { get; set; }
        public string? Status { get; set; }
    }
}
