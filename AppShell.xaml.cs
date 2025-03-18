using DentalApp.Services;

namespace DentalApp
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();
            //this.Loaded += OnShellLoaded; // Triggered when Shell is fully loaded
        }


        private async void OnShellLoaded(object sender, EventArgs e)
        {
            await Task.Delay(50); // Small delay to ensure UI is ready

            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            bool isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;

            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;

            if (!isInternetAvailable && !isApiAvailable)
            {
                App.Instance.IsConnected = false;
                await DisplayAlert("No Connection", "No Internet and API connection", "OK"); 
            }
            else if (isInternetAvailable && !isApiAvailable)
            {
                App.Instance.IsConnected = false;
                await DisplayAlert("API Unreachable", "Internet Connected but API unreachable", "OK");
            }
            else if (!isInternetAvailable && isApiAvailable)
            {
                App.Instance.IsConnected = true;
                await DisplayAlert("Limited Access", "API connected but no Internet connection", "OK");
            }
            //else if (isInternetAvailable && isApiAvailable)
            //{
            //    App.Instance.IsConnected = true;
            //    await DisplayAlert("Fully online", $"Value: {App.Instance.IsConnected}", "OK");
            //}

        }
    }
}
