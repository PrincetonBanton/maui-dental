using DentalApp.Services.ApiServices;
using System.Net.Http.Json;

namespace DentalApp.Services
{
    public class DashboardService : BaseApiService
    {
        //private const string BaseUrl = "https://localhost:7078";
        //private readonly HttpClient _httpClient = new();

        //public DashboardService()
        //{
        //    _ = TokenService.AttachTokenAsync(_httpClient);
        //}

        public async Task<decimal> GetTotalIncomeAsync(DateTime startDate, DateTime endDate)
        {
            string url = $"{BaseUrl}/Dashboard/GetTotalIncome?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<decimal>(url);
        }

        public async Task<decimal> GetTotalExpenseAsync(DateTime startDate, DateTime endDate)
        {
            string url = $"{BaseUrl}/Dashboard/GetTotalExpense?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}";
            return await _httpClient.GetFromJsonAsync<decimal>(url);
        }
    }
}

