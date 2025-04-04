using System;
using System.Collections.Generic;

namespace DentalApp.Models
{
    public class SaleCreate
    {
        public string SaleNo { get; set; }
        public DateTime SaleDate { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<SaleItemCreate> Items { get; set; } = new();
        public PaymentCreate Payment { get; set; } = new();
    }

    public class SaleItemCreate
    {
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
        public int ProductId { get; set; }
    }

    public class PaymentCreate
    {
        public decimal PaymentAmount { get; set; }
        public int PaymentType { get; set; }
        public decimal AmountTendered { get; set; }
        public int EnteredBy { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
