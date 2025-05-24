using DentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;

namespace DentalApp.Models
{
    public partial class Sale : BaseModel
    {
        public DateTime SaleDate { get; set; }
        public string? SaleNo { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int PatientId { get; set; }
        public int DentistId { get; set; }

        [Precision(9, 2)]
        public decimal SubTotal { get; set; }

        [Precision(9, 2)]
        public decimal Total { get; set; }

        [Precision(9, 2)]
        public decimal? AmountDue { get; set; }
        public string? Note { get; set; }

        public virtual Dentist? Dentist { get; set; }
        public virtual Patient? Patient { get; set; }
        public virtual ICollection<Payment> Payments { get; set; } = new HashSet<Payment>();
        public virtual ICollection<SaleLine> SaleLines { get; set; } = new HashSet<SaleLine>();
    }
}
