using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Text.Json;

namespace DentalApp.Services
{
    public static class TokenService
    {
        public static async Task AttachTokenAsync(HttpClient httpClient)
        {
            if (Shell.Current == null) return;

            var token = Preferences.Get("AuthToken", string.Empty);
            if (string.IsNullOrEmpty(token))
            {
                await Shell.Current.DisplayAlert("Token", "No token found in Preferences", "OK");
                return;
            }

            try
            {
                if (IsTokenExpired(token))
                {
                    await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please log in again.", "OK");
                    Preferences.Remove("AuthToken");
                    Application.Current.MainPage = new NavigationPage(new Pages.LoginPage());
                    return;
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Token Error", $"Error: {ex.Message}", "OK");
            }
        }


        public static bool IsTokenExpired(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            if (!handler.CanReadToken(token)) return true;

            var jwtToken = handler.ReadJwtToken(token);
            var exp = jwtToken.Payload.Exp;

            if (exp == null) return true;

            // Convert to DateTime
            var expirationTime = DateTimeOffset.FromUnixTimeSeconds(exp.Value).UtcDateTime;
            return expirationTime < DateTime.UtcNow;
        }
    }
}
