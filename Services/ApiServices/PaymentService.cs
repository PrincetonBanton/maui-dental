using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class PaymentService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public PaymentService()
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

        // Payment methods
        public Task<bool> AddPaymentAsync(Payment payment)
            => PostAsync("Payment/AddPayment", payment);

        public Task<List<Payment>> GetPaymentsAsync(int patientId)
            => GetAsync<List<Payment>>($"Payment/GetPayments/{patientId}");
    }
}
