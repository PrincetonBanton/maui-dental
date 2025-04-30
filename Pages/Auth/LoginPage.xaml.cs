using DentalApp.Services;
using System.Text.Json;

namespace DentalApp.Pages.Auth;

public partial class LoginPage : ContentPage
{
    private readonly ApiService _apiService = new();

    public LoginPage()
    {
        InitializeComponent();
    }

    private async void OnLoginClicked(object sender, EventArgs e)
    {
        string username = UsernameEntry.Text?.Trim();
        string password = PasswordEntry.Text;

        ErrorLabel.IsVisible = false;

        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            ErrorLabel.Text = "Please enter both username and password.";
            ErrorLabel.IsVisible = true;
            return;
        }

        try
        {
            var token = await _apiService.AuthenticateAsync(username, password);

            if (!string.IsNullOrWhiteSpace(token))
            {
                Preferences.Set("AuthToken", token);
                Application.Current.MainPage = new AppShell(); // Navigate to main app
            }
            else
            {
                ErrorLabel.Text = "Invalid username or password.";
                ErrorLabel.IsVisible = true;
            }
        }
        catch (Exception ex)
        {
            ErrorLabel.Text = "Login failed. Try again.";
            ErrorLabel.IsVisible = true;
            Console.WriteLine(ex.Message);
        }
    }
}
