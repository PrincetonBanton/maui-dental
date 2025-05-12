using DentalApp.Services;
using System.Text.Json;
namespace DentalApp.Pages.Auth;

public partial class LoginPage : ContentPage
{
    private readonly AuthService _authService = new(); 

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text;
        string password = PasswordEntry.Text;

        var tokenResponse = await _authService.AuthenticateAsync(username, password);

        // Example if tokenResponse is a JSON string:
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
            UsernameEntry.Text = string.Empty;
            PasswordEntry.Text = string.Empty;
            await DisplayAlert("Login Failed", "Invalid username or password.", "OK");
        }
    }
}
