using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class ProductVM : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public double Amount { get; set; } = 0.00;
        public double MinPrice { get; set; } = 0.00;
        public double MaxPrice { get; set; } = 0.00;
        public ProductType ProductType { get; set; }
    }
}
