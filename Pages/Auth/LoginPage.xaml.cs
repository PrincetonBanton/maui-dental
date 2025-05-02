using DentalApp.Services;

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

        var token = await _authService.AuthenticateAsync(username, password);

        if (!string.IsNullOrEmpty(token))
        {
            Preferences.Set("AuthToken", token);
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
