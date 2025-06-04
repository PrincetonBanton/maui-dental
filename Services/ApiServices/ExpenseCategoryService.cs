using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class ExpenseCategoryService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public ExpenseCategoryService()
        {
            _ = TokenService.AttachTokenAsync(_httpClient);
        }

        private async Task<T?> RequestAsync<T>(Func<Task<T?>> request, string errorMessage)
        {
            try
            {
                return await request();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"{errorMessage}: {ex.Message}", "OK");
                return default;
            }
        }

        private Task<T?> GetAsync<T>(string endpoint)
            => RequestAsync(() => _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}"), "Error fetching data");

        private Task<bool> PostAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error creating data");

        private Task<bool> PutAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error updating data");

        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");

        // Expense Categories
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
