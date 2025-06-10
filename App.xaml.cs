using Microsoft.Maui.Storage;
using DentalApp.Services;

namespace DentalApp
{
    public partial class App : Application
    {
        public static App Instance { get; private set; }

        // Global navigation state variables
        public string UserNavigated { get; set; } = "start";
        public string PatientNavigated { get; set; } = "start";
        public string DentistNavigated { get; set; } = "start";
        public bool IsConnected { get; set; } = true;

        public App()
        {
            InitializeComponent();
            Instance = this;

            //var token = Preferences.Get("AuthToken", string.Empty);

            //if (!string.IsNullOrEmpty(token) && !TokenService.IsTokenExpired(token))
            //{
            //    MainPage = new AppShell();
            //}
            //else
            //{
            //    MainPage = new NavigationPage(new Pages.LoginPage());
            //}

            MainPage = new NavigationPage(new Pages.LoginPage());
        }
        protected override void OnSleep()
        {
            Preferences.Remove("AuthToken");
            base.OnSleep();
        }
    }
}
