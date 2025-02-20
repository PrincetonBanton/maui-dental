using DentalApp.Models;

namespace DentalApp.Data
{
    public static class SampleData
    {
        public static List<User> GetSampleUsers()
        {
            return new List<User>
            {
                new User { Id = 1, FirstName = "John", MiddleName = "D", LastName = "Offline", Email = "john@example.com", Mobile = "123-456-7890", RoleName = "Dentist" },
                new User { Id = 2, FirstName = "Jane", MiddleName = "E", LastName = "Smith", Email = "jane@example.com", Mobile = "987-654-3210", RoleName = "Patient" },
                new User { Id = 3, FirstName = "Mark", MiddleName = "F", LastName = "Johnson", Email = "mark@example.com", Mobile = "555-666-7777", RoleName = "Staff" }
            };
        }

        public static List<Product> GetSampleProducts()
        {
            return new List<Product>
            {
                new Product { Id = 1, Name = "Mouthwash", Description = "Offline", Amount = 150, ProductType = 1 },
                new Product { Id = 2, Name = "Teeth Cleaning", Description = "Professional dental cleaning", Amount = 500, ProductType = 2 },
                new Product { Id = 3, Name = "Tooth Extraction", Description = "Painless tooth extraction", Amount = 500, ProductType = 2 },
                new Product { Id = 4, Name = "Toothpaste", Description = "Fluoride toothpaste", Amount = 100, ProductType = 1 },
            };
        }

    }
}
