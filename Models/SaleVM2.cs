using System;
using System.Collections.Generic;

namespace DentalApp.Models
{
    public class SaleVM2 : BaseModel
    {
        public string SaleNo { get; set; }
        public DateTime SaleDate { get; set; }
        public int PatientId { get; set; }
        public int DentistId { get; set; }
        public string Note { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Total { get; set; }
        public List<SaleLine> Items { get; set; }
    }
}
