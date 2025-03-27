using System;
using System.Collections.Generic;

namespace DentalApp.Models
{
    public class SaleVM2
    {
        public string SaleNo { get; set; }
        public DateTime SaleDate { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<SaleItemVM> Items { get; set; }
        public PaymentVM Payment { get; set; }
    }

    public class SaleItemVM
    {
        public string Name { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Amount { get; set; }
    }

    public class PaymentVM
    {
        public decimal PaymentAmount { get; set; }
        public int PaymentType { get; set; }
        public decimal AmountTendered { get; set; }
        public int EnteredBy { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
