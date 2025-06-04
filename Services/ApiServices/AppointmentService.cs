using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class AppointmentService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public AppointmentService()
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

        // Appointment Methods
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            var appointments = await GetAsync<List<Appointment>>("Appointment/GetAll") ?? new List<Appointment>();
            return appointments.OrderByDescending(e => e.Id).ToList();
        }

        public Task<bool> CreateAppointmentAsync(Appointment appointment)
            => PostAsync("Appointment/Create", appointment);

        public Task<bool> UpdateAppointmentAsync(Appointment appointment)
            => PutAsync($"Appointment/Update/{appointment.Id}", appointment);

        public Task<bool> DeleteAppointmentAsync(int id)
            => DeleteAsync($"Appointment/Delete/{id}");
    }
}
    