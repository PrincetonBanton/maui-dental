namespace DentalApp.Services
{
    public class ApiConnectivityService
    {
        private static readonly ApiConnectivityService _instance = new();
        public static ApiConnectivityService Instance => _instance;
        private readonly HttpClient _httpClient;

        private ApiConnectivityService()
        {
            _httpClient = new HttpClient();
        }

        public bool IsApiAvailable { get; private set; } = false;

        public async Task CheckApiConnectivityAsync()
        {
            try
            {
                string apiUrl = "https://localhost:7078/swagger/index.html"; // Change to your actual API URL
                HttpResponseMessage response = await _httpClient.GetAsync(apiUrl);
                IsApiAvailable = response.IsSuccessStatusCode;
            }
            catch
            {
                IsApiAvailable = false;
            }
        }
    }
}
