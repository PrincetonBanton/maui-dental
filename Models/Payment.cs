using DentalApp .Models;
using DentalApp.Models.Enum;
using Microsoft.EntityFrameworkCore;
using System;

namespace DentalApp.Models
{
    public partial class Payment : BaseModel
    {
        [Precision(9, 2)]
        public decimal PaymentAmount { get; set; }
        public PaymentType PaymentType { get; set; } 
        public int SaleId { get; set; }

        [Precision(9, 2)]
        public decimal AmountTendered { get; set; }
        public int? EnteredBy { get; set; }
        public PaymentStatus Status { get; set; } = PaymentStatus.Confirmed;
        public DateTime PaymentDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;

        public string SaleProductName { get; set; }
        public virtual Sale? Sale { get; set; }
    }
}
