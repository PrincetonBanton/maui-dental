using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Storage;
using System.IdentityModel.Tokens.Jwt;

namespace DentalApp.Services
{
    public static class TokenService
    {
        public static async Task AttachTokenAsync(HttpClient httpClient)
        {
            if (Shell.Current == null) return;

            var tokenJson = Preferences.Get("AuthToken", string.Empty);
            if (string.IsNullOrEmpty(tokenJson))
            {
                await Shell.Current.DisplayAlert("Token", "No token found in Preferences", "OK");
                return;
            }

            try
            {
                var tokenObject = JsonSerializer.Deserialize<Dictionary<string, object>>(tokenJson);
                var tokenResult = tokenObject?.GetValueOrDefault("result")?.ToString();

                if (string.IsNullOrEmpty(tokenResult))
                {
                    await Shell.Current.DisplayAlert("Token Error", "Result field is empty in the token", "OK");
                    return;
                }

                if (IsTokenExpired(tokenResult))
                {
                    await Shell.Current.DisplayAlert("Session Expired", "Your session has expired. Please log in again.", "OK");
                    Preferences.Remove("AuthToken");
                    Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
                }

                httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", tokenResult);
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Deserialization Error", $"Error: {ex.Message}", "OK");
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
