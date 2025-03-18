using SQLite;

namespace DentalApp.Models
{
    public class Expense : BaseModel
    {
        public string? Description { get; set; }

        //[Precision(9, 2)]
        public decimal Amount { get; set; }
        public DateTime ExpenseDate { get; set; }
        public DateTime CreatedOn { get; set; } = DateTime.Now;
        public int EnteredBy { get; set; }
        public int ExpenseCategoryId { get; set; }

        public virtual ExpenseCategory? ExpenseCategory { get; set; }
    }
}
