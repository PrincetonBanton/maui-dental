using DentalApp.Models.Enum;

namespace DentalApp.Models
{
    public class ProductVM : BaseModel
    {
        public string? Name { get; set; }
        public string? Description { get; set; }
        public string? ProductCode { get; set; }
        public double Amount { get; set; }
        public double? MinPrice { get; set; }
        public double? MaxPrice { get; set; }
        public ProductType ProductType { get; set; }
    }
}
