using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class ExpenseService : BaseApiService
    {
        public async Task<List<Expense>> GetExpensesAsync()
        {
            var expenses = await GetAsync<List<Expense>>("Expense/GetAll") ?? new List<Expense>();
            return expenses.OrderByDescending(e => e.Id).ToList();
        }

        public Task<bool> CreateExpenseAsync(Expense expense)
            => PostAsync("Expense/Create", expense);

        public Task<bool> UpdateExpenseAsync(Expense expense)
            => PutAsync($"Expense/Update/{expense.Id}", expense);

        public Task<bool> DeleteExpenseAsync(int id)
            => DeleteAsync($"Expense/Delete/{id}");
    }
}
