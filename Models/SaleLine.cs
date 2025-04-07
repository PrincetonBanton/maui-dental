using DentalApp.Models;
using Microsoft.EntityFrameworkCore;

namespace DentalApp.Models
{
    public partial class SaleLine : BaseModel
    {
        public int SaleId { get; set; }
        public int Quantity { get; set; }

        [Precision(9, 2)]
        public decimal SubTotal { get; set; }

        [Precision(9, 2)]
        public decimal Total { get; set; }
        public int ProductId { get; set; }
        public string? ProductName { get; set; }  
        public virtual Product? Product { get; set; }
        public virtual Sale? Sale { get; set; }
    }
}
