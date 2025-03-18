using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class ProductVM : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public decimal Amount { get; set; } = 0.00m;
        public decimal MinPrice { get; set; } = 0.00m;
        public decimal MaxPrice { get; set; } = 0.00m;
        public ProductType ProductType { get; set; }
    }
}
