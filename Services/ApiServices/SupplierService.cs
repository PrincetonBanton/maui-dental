using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class SupplierService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public SupplierService()
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

        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            var suppliers = await GetAsync<List<Supplier>>("Supplier/GetAll") ?? new List<Supplier>();
            return suppliers.OrderByDescending(s => s.Id).ToList();
        }

        public Task<bool> CreateSupplierAsync(Supplier supplier) => PostAsync("Supplier/Create", supplier);

        public Task<bool> UpdateSupplierAsync(Supplier supplier) => PutAsync($"Supplier/Update/{supplier.Id}", supplier);

        public Task<bool> DeleteSupplierAsync(int id) => DeleteAsync($"Supplier/Delete/{id}");
    }
}
