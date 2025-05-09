using System.Text;
using System.Text.Json;

namespace DentalApp.Services
{
    public class AuthService
    {
        private const string BaseUrl = "https://localhost:7078";  // API base URL
        private readonly HttpClient _httpClient = new();

        public async Task<string?> AuthenticateAsync(string username, string password)
        {
            var payload = new { username, password };
            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            try
            {
                var response = await _httpClient.PostAsync($"{BaseUrl}/Account/Login", content);
                if (!response.IsSuccessStatusCode)
                    return null;

                return await response.Content.ReadAsStringAsync(); // return full JSON string
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Authentication error: {ex.Message}");
                return null;
            }
        }
    }
}
