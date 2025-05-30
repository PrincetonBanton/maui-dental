using DentalApp.Models;

namespace DentalApp.Services.Validations
{
    public static class ExpenseValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateExpense(Expense expense)
        {
            if (string.IsNullOrWhiteSpace(expense.Description))
                return (false, "Expense description is required.");

            if (expense.Amount <= 0)
                return (false, "Amount must be a positive number.");

            if (expense.ExpenseCategoryId <= 0)
                return (false, "Please select a valid expense category.");

            if (expense.EnteredBy <= 0)
                return (false, "Invalid user. Please check the entered by field.");

            if (expense.ExpenseDate == default)
                return (false, "Please select a valid expense date."); ;

            return (true, string.Empty);
        }
        public static (bool IsValid, string ErrorMessage) ValidateCategory(ExpenseCategory category)
        {
            if (string.IsNullOrWhiteSpace(category.Name))
                return (false, "Category Name is required.");
            if (string.IsNullOrWhiteSpace(category.Description))
                return (false, "Category description is required.");

            return (true, string.Empty);
        }
    }
}
