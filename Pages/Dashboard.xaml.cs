using DentalApp.Services; // Ensure you have access to your ApiService or other related services

namespace DentalApp.Pages
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("AuthToken");

            // You can also call an API method to log out on the server if needed
            // await _apiService.LogoutAsync();

            Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        }
    }
}
