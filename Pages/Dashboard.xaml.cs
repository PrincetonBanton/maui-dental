using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
        }

        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();
        //    // Check if the token is expired when the page appears
        //    var tokenJson = Preferences.Get("AuthToken", string.Empty);
        //    if (string.IsNullOrEmpty(tokenJson) || TokenService.IsTokenExpired(tokenJson))
        //    {
        //        // If no token or token is expired, log out and navigate to login page
        //        Preferences.Remove("AuthToken");
        //        Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        //    }
        //}

        private async void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("AuthToken");
            Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        }
    }
}
