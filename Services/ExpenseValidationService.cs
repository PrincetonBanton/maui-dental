using DentalApp.Models;

namespace DentalApp.Services
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
    }
}
