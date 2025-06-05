using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class ExpenseCategoryService : BaseApiService
    {
        public Task<List<ExpenseCategory>> GetExpenseCategoriesAsync()
            => GetAsync<List<ExpenseCategory>>("Expense/GetCategories") ?? Task.FromResult(new List<ExpenseCategory>());

        public Task<bool> CreateExpenseCategoryAsync(ExpenseCategory category)
            => PostAsync("Expense/CreateExpenseCategory", category);

        public Task<bool> UpdateExpenseCategoryAsync(ExpenseCategory category)
            => PutAsync($"Expense/UpdateExpenseCategory/{category.Id}", category);

        public Task<bool> DeleteExpenseCategoryAsync(int id)
            => DeleteAsync($"Expense/DeleteExpenseCategory/{id}");
    }
}
