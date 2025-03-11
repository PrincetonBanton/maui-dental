using DentalApp.Models;
using DentalApp.Models.Enum;

namespace DentalApp.Data
{
    public static class SampleData
    {
        public static List<UserVM> GetSampleUsers()
        {
            return new List<UserVM>
            {
                new UserVM { Id = 1, FirstName = "John", MiddleName = "D", LastName = "Offline", Email = "john@example.com", Mobile = "123-456-7890", RoleName = "Dentist" },
                new UserVM { Id = 2, FirstName = "Jane", MiddleName = "E", LastName = "Smith", Email = "jane@example.com", Mobile = "987-654-3210", RoleName = "Patient" },
                new UserVM { Id = 3, FirstName = "Mark", MiddleName = "F", LastName = "Johnson", Email = "mark@example.com", Mobile = "555-666-7777", RoleName = "Staff" }
            };
        }
        public static List<PatientVM> GetSamplePatients()
        {
            return new List<PatientVM>
            {
                new PatientVM { Id = 1, FirstName = "John", MiddleName = "D", LastName = "Offline", Email = "john@example.com", Mobile = "123-456-7890" },
                new PatientVM { Id = 2, FirstName = "Jane", MiddleName = "E", LastName = "Smith", Email = "jane@example.com", Mobile = "987-654-3210" },
                new PatientVM { Id = 3, FirstName = "Mark", MiddleName = "F", LastName = "Johnson", Email = "mark@example.com", Mobile = "555-666-7777" }
            };
        }
        public static List<DentistVM> GetSampleDentists()
        {
            return new List<DentistVM>
            {
                new DentistVM { Id = 1, FirstName = "John", MiddleName = "D", LastName = "Offline", Email = "john@example.com", Mobile = "123-456-7890" },
                new DentistVM { Id = 2, FirstName = "Jane", MiddleName = "E", LastName = "Smith", Email = "jane@example.com", Mobile = "987-654-3210" },
                new DentistVM { Id = 3, FirstName = "Mark", MiddleName = "F", LastName = "Johnson", Email = "mark@example.com", Mobile = "555-666-7777" }
            };
        }
        public static List<ProductVM> GetSampleProducts()
        {
            return new List<ProductVM>
            {
                new ProductVM { Id = 1, Name = "Offline Name", Description = "Offline", Amount = 150, ProductType = ProductType.Goods },
                new ProductVM { Id = 2, Name = "Teeth Cleaning", Description = "Professional dental cleaning", Amount = 500, ProductType = ProductType.Goods },  
                new ProductVM { Id = 3, Name = "Tooth Extraction", Description = "Painless tooth extraction", Amount = 500,  ProductType = ProductType.Goods },
                new ProductVM { Id = 4, Name = "Toothpaste", Description = "Fluoride toothpaste", Amount = 100, ProductType = ProductType.Goods },
            };
        }

    }
}
