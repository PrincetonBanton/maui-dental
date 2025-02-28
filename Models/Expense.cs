using SQLite;

namespace DentalApp.Models
{
    public class Expense : BaseModel
    {
        public string? Description { get; set; }
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int EnteredBy { get; set; }
        public int ExpenseCategoryId { get; set; }

        [Ignore]
        public ExpenseCategory ExpenseCategory { get; set; }

        [Ignore]
        public string EnteredByName { get; set; }
    }
}
