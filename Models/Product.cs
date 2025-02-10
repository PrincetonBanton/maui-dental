using SQLite;

namespace DentalApp.Models
{
    public class Product
    {
        [PrimaryKey, AutoIncrement]
        public int Id { get; set; }

        public string? Name { get; set; } 
        public string? Description { get; set; } 
        public double Amount { get; set; }
        public int ProductType { get; set; }  
        public bool? Enabled { get; set; }  
        public string? ProductCode { get; set; } 
        public double? MinPrice { get; set; }  
        public double? MaxPrice { get; set; } 

        [Ignore]
        public string ProductImage => ProductType == 1 ? "products1.png" : "services1.png";
    }
}
