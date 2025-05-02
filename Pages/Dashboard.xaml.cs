using DentalApp.Services; 

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
            Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        }
    }
}
