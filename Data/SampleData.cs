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
                new ProductVM { Id = 2, Name = "Sample Cleaning", Description = "Professional dental cleaning", Amount = 500, ProductType = ProductType.Goods },  
                new ProductVM { Id = 3, Name = "Offline Extraction", Description = "Painless tooth extraction", Amount = 500,  ProductType = ProductType.Goods },
                new ProductVM { Id = 4, Name = "SampleName", Description = "Fluoride toothpaste", Amount = 100, ProductType = ProductType.Goods },
            };
        }
        public static List<Expense> GetSampleExpenses()
        {
            return new List<Expense>
            { 
                new Expense { Id = 1, Description = "Office Supplies", Amount = 150.00m, ExpenseDate = DateTime.Now, CreatedOn = DateTime.Now, EnteredBy = 1, ExpenseCategoryId = 2 },
                new Expense { Id = 2, Description = "Dental Cleaning Equipment", Amount = 500.00m, ExpenseDate = DateTime.Now.AddDays(-2), CreatedOn = DateTime.Now, EnteredBy = 2, ExpenseCategoryId = 3 },
                new Expense { Id = 3, Description = "Tooth Extraction Tools", Amount = 750.00m, ExpenseDate = DateTime.Now.AddDays(-5), CreatedOn = DateTime.Now, EnteredBy = 3, ExpenseCategoryId = 3 },
                new Expense { Id = 4, Description = "Fluoride Treatment Supplies", Amount = 120.00m, ExpenseDate = DateTime.Now.AddDays(-7), CreatedOn = DateTime.Now, EnteredBy = 4, ExpenseCategoryId = 1 },
            };

        }
        public static List<SaleVM> GetSampleSales()
        {
            return new List<SaleVM>
            {
                new SaleVM { SaleId = 1, SaleDate = DateTime.Now.AddDays(-1), PatientName = "Johannes Elmer Arrocena", DentistName = "Dr. Jane Smith", Total = 1500.00m, Status = "Unpaid" },
                new SaleVM { SaleId = 2, SaleDate = DateTime.Now.AddDays(-3), PatientName = "Alice Brown", DentistName = "Dr. Mark Johnson", Total = 1200.00m, Status = "Unpaid" },
                new SaleVM { SaleId = 3, SaleDate = DateTime.Now.AddDays(-7), PatientName = "Michael Scott", DentistName = "Dr. John Doe", Total = 1800.00m, Status = "Cancelled" },
                new SaleVM { SaleId = 4, SaleDate = DateTime.Now.AddDays(-10), PatientName = "Pam Beesly", DentistName = "Dr. Sarah Connor", Total = 900.00m, Status = "Paid" }
            };
        }

    }
}
