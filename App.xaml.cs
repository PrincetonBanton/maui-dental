using Microsoft.Maui.Storage;

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

            // Check if user is logged in
            var token = Preferences.Get("AuthToken", string.Empty);

            //MainPage = new AppShell();

            if (!string.IsNullOrEmpty(token))
            {
                MainPage = new AppShell(); // User is logged in
            }
            else
            {
                MainPage = new NavigationPage(new Pages.Auth.LoginPage()); // Show login page
            }
        }
    }
}
