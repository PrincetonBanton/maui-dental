using DentalApp.Services;
using DentalApp.Pages;
using Microsoft.Maui.Controls;

namespace DentalApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            this.Loaded += OnShellLoaded; // Triggered when Shell is fully loaded

            Routing.RegisterRoute(nameof(ProductListPage), typeof(ProductListPage));
            Routing.RegisterRoute(nameof(UserListPage), typeof(UserListPage));
            Routing.RegisterRoute(nameof(PatientListPage), typeof(PatientListPage));
            Routing.RegisterRoute(nameof(DentistListPage), typeof(DentistListPage));
            Routing.RegisterRoute(nameof(ExpensePage), typeof(ExpensePage));

            RegisterNavigationEvents();
        }


        private async void OnShellLoaded(object sender, EventArgs e)
        {
            await Task.Delay(500); // Small delay to ensure UI is ready

            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            bool isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;

            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;

            if (!isInternetAvailable && !isApiAvailable)
            {
                await DisplayAlert("No Connection", "You are offline and cannot reach the API.", "OK"); 
            }
            else if (!isInternetAvailable && isApiAvailable)
            {
                await DisplayAlert("Limited Access", "You have no internet, but can still reach the API.", "OK");
            }
            else if (isInternetAvailable && !isApiAvailable)
            {
                await DisplayAlert("API Unreachable", "You are online, but cannot connect to the API.", "OK");
            }
        }
        private void RegisterNavigationEvents()
        {
            this.Navigating += async (s, e) =>
            {
                if (e.Target.Location.OriginalString.Contains("UserListPage"))
                {
                    await DisplayAlert("Navigation", $"UserNavigated: {App.Instance.UserNavigated}", "OK");
                }
            };
        }

    }
}
