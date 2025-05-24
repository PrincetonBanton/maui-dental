using SQLite;

namespace DentalApp.Models
{
    public class ExpenseCategory : BaseModel
    {
        public string? Name { get; set; }
        public string Description { get; set; }
    }
}
