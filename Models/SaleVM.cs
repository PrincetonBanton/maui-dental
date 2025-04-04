using System;

namespace DentalApp.Models
{
    public class SaleVM : BaseModel
    {
        public int SaleId { get; set; }  // Only for existing sales
        public string SaleNo { get; set; }
        public DateTime SaleDate { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }  // Only for viewing sales
        public int DentistId { get; set; }
        public string? DentistName { get; set; }  // Only for viewing sales
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal AmountDue { get; set; } // For tracking unpaid amounts
        public string? Status { get; set; }    // e.g., "Unpaid", "Completed"

        public List<SaleItem> Items { get; set; } = new();
        public List<SalePayment> Payments { get; set; } = new();

        public class SaleItem  // Merging SaleItemCreate & SaleLine
        {
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Amount { get; set; } // Unified field for price
        }

        public class SalePayment  // Merging PaymentCreate & Payment
        {
            public decimal PaymentAmount { get; set; }
            public int PaymentType { get; set; }
            public decimal AmountTendered { get; set; }
            public int EnteredBy { get; set; }
            public DateTime PaymentDate { get; set; }
        }
    }
}

