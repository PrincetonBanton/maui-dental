using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services.ApiServices
{
    public class UserService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        public UserService()
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

        // Roles
        public Task<List<Role>> GetRolesAsync()
            => GetAsync<List<Role>>("Role/GetAll") ?? Task.FromResult(new List<Role>());

        // Users
        public async Task<List<UserVM>> GetUsersAsync()
        {
            var users = await GetAsync<List<UserVM>>("User/GetAll") ?? new List<UserVM>();
            return users;
        }

        public Task<UserVM?> GetUserByIdAsync(int userId)
            => GetAsync<UserVM>($"User/Get/{userId}");

        public Task<bool> CreateUserAsync(UserVM user)
            => PostAsync("Account/Register", user);

        public Task<bool> UpdateUserAsync(UserVM user)
            => PutAsync($"User/Update/{user.Id}", user);

        public Task<bool> DeleteUserAsync(int id)
            => DeleteAsync($"User/Delete/{id}");
    }
}
