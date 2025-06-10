using DentalApp.Services;
using System.Text.Json;

namespace DentalApp.Pages;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService = new(); 

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        // Check API connectivity
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();

        if (!ApiConnectivityService.Instance.IsApiAvailable)
        {
            await DisplayAlert("Connection Error", "Cannot connect to the server. Please check your internet connection or try again later.", "OK");
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            return;
        }

        //Try to authenticate
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        string tokenResponse = null;

        try
        {
            tokenResponse = await _authService.AuthenticateAsync(username, password);
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to authenticate. {ex.Message}", "OK");
            return;
        }

        if (string.IsNullOrWhiteSpace(tokenResponse))
        {
            await DisplayAlert("Login Failed", "Error on authentication. Please try again", "OK");
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            return;
        }

        // Parse the response
        try
        {
            var tokenObject = JsonSerializer.Deserialize<Dictionary<string, object>>(tokenResponse);
            var token = tokenObject?.GetValueOrDefault("result")?.ToString();

            if (!string.IsNullOrEmpty(token))
            {
                Preferences.Set("AuthToken", token);
                using var httpClient = new HttpClient();
                await TokenService.AttachTokenAsync(httpClient);
                Application.Current.MainPage = new AppShell();
            }
            else
            {
                await DisplayAlert("Login Failed", "Error on authentication. Please try again", "OK");
                UsernameEntry.Text = string.Empty;
                PasswordEntry.Text = string.Empty;
            }
        }
        catch (JsonException)
        {
            await DisplayAlert("Login Failed", "Unexpected server response. Please try again later.", "OK");
        }
    }


}
