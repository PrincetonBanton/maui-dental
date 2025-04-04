using System;

namespace DentalApp.Models
{
    public class SaleVM : BaseModel
    {
        public DateTime SaleDate { get; set; }
        public int SaleId { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }
        public int DentistId { get; set; }
        public string? DentistName { get; set; }
        public string? Status { get; set; }
        public string SaleNo { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal AmountDue { get; set; }
        public List<SaleLine> Items { get; set; } = new();
        public List<Payment> Payment { get; set; } = new();
    }
}

