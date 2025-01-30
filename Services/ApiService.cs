using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        // Generic method to handle requests
        private async Task<T?> RequestAsync<T>(Func<Task<T?>> request, string errorMessage)
        {
            try
            {
                return await request();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{errorMessage}: {ex.Message}");
                return default;
            }
        }

        // GET request
        private Task<T?> GetAsync<T>(string endpoint)
            => RequestAsync(() => _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}"), "Error fetching data");

        // POST request
        private Task<bool> PostAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error creating data");

        // PUT request
        private Task<bool> PutAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error updating data");

        // DELETE request
        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");


        // User Methods
        public Task<List<User>> GetUsersAsync()
            => GetAsync<List<User>>("User/GetAll") ?? Task.FromResult(new List<User>());

        public Task<User?> GetUserByIdAsync(int id)
            => GetAsync<User>($"User/Get/{id}");
        public Task<bool> CreateUserAsync(User user)
            => PostAsync("User/Create", user);
        public Task<bool> UpdateUserAsync(User user)
            => PutAsync($"User/Update/{user.Id}", user);
        public Task<bool> DeleteUserAsync(int id)
            => DeleteAsync($"User/Delete/{id}");
        public Task<bool> RegisterUserAsync(User user)
            => PostAsync("Account/Register", user);

        //Role
        public Task<List<Role>> GetRolesAsync()
            => GetAsync<List<Role>>("Role/GetAll") ?? Task.FromResult(new List<Role>());
    }
}
