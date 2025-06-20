﻿using System;
using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class SaleVM : BaseModel
    {
        public int SaleId { get; set; }          // Only for existing sales
        public string? SaleNo { get; set; }
        public DateTime SaleDate { get; set; }
        public int PatientId { get; set; }
        public string? PatientName { get; set; }  // Only for viewing sales
        public int DentistId { get; set; }
        public string? DentistName { get; set; }  // Only for viewing sales
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public decimal TotalDue { get; set; }
        public string PartialDueText
        {
            get
            {
                return Status == 1
                    ? $"{Total:N2} ({TotalDue:N2})"
                    : $"{Total:N2}";
            }
        }

        public decimal AmountDue { get; set; } // For tracking unpaid amounts
        public short Status { get; set; }    // e.g., "Unpaid", "Completed"
        public string StatusText => ((SaleStatus)Status).ToString().Replace("_", " ");

        public List<SaleItem> Items { get; set; } = new();
        public List<SalePayment> Payments { get; set; } = new();

        public class SaleItem  // Merging SaleItemCreate & SaleLine
        {
            public int? Id { get; set; }  // Allowing null for the ID
            public int ProductId { get; set; }
            public int Quantity { get; set; }
            public decimal Amount { get; set; } = 0.00m;
            public decimal? SubTotal { get; set; }      // Updated to match API structure
            public decimal? Total { get; set; }         // Updated to match API structure
            public string? ProductName { get; set; }    // Allowing null for ProductName
            public int? SaleId { get; set; }            // Allowing null for SaleId
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

