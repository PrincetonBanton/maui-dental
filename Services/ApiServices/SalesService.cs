using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class SaleService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public SaleService()
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

        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");

        // Sale methods
        public async Task<List<SaleVM>> GetSalesAsync()
        {
            var sales = await GetAsync<List<SaleVM>>("Sale/GetAll") ?? new List<SaleVM>();
            return sales.OrderByDescending(e => e.SaleId).ToList();
        }

        public Task<SaleVM?> GetSaleDetailAsync(int id)
            => GetAsync<SaleVM>($"Sale/GetDetail/{id}");

        public Task<bool> CreateSaleAsync(SaleVM sale)
            => PostAsync("Sale/Create", sale);

        public Task<bool> DeleteSaleAsync(int id)
            => DeleteAsync($"Sale/Delete/{id}");
    }
}
