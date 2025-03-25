using DentalApp.Models.Enum;

namespace DentalApp.Models.ViewModels
{
    public class ProductVM
    {
        public int Id { get; set; }

        public string? Name { get; set; }

        public string? Description { get; set; }

        public string? ProductCode { get; set; }

        public decimal Amount { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public ProductType ProductType { get; set; }
        public bool Enabled { get; set; }
    }
}
