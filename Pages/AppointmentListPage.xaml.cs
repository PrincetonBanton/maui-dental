using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Text.Json;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class AppointmentListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private ObservableCollection<Appointment> _allAppointments = new();
        private bool _isLandscape = false;

        public AppointmentListPage()
        {
            InitializeComponent();
            LoadAppointmentList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateOrientation(Width, Height);
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateOrientation(width, height); // Also check when size changes
        }

        private void UpdateOrientation(double width, double height)
        {
            bool newIsLandscape = width > height;
            _isLandscape = newIsLandscape;

            var templateKey = _isLandscape ? "LandscapeTemplate" : "PortraitTemplate";
            var template = (DataTemplate)this.Resources[templateKey];
            AppointmentListView.ItemTemplate = template;

            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    await DisplayAlert("Orientation Changed", _isLandscape ? "Landscape" : "Portrait", "OK");
            //});
        }

        private async void LoadAppointmentList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;

            try
            {
                var appointmentList = isApiAvailable
                    ? await _apiService.GetAppointmentsAsync() ?? new List<Appointment>()
                    : SampleData.GetSampleAppointments();

                _allAppointments.Clear();
                appointmentList.ForEach(_allAppointments.Add);
                AppointmentListView.ItemsSource = _allAppointments;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load appointments. Please try again.", "OK");
            }
        }


        private async void OnCreateAppointmentButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new AppointmentDetailsPage()); // Rename ProductDetailsPage to AppointmentDetailsPage
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            //if (sender is ImageButton button && button.BindingContext is ProductVM selectedAppointment)
            //{
            //    await Navigation.PushAsync(new AppointmentDetailsPage(selectedAppointment));
            //}
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            //if (sender is ImageButton button && button.BindingContext is ProductVM selectedAppointment)
            //{
            //    bool confirmDelete = await DisplayAlert("Confirm", "Delete this appointment?", "Yes", "No");
            //    if (!confirmDelete) return;

            //    var success = await _apiService.DeleteProductAsync(selectedAppointment.Id);
            //    LoadAppointmentList();
            //    await DisplayAlert(success ? "Success" : "Error", success ? "Appointment deleted." : "Failed to delete appointment.", "OK");
            //}
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            //var searchText = e.NewTextValue.ToLower();

            //AppointmentListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
            //    ? _allAppointments
            //    : _allAppointments.Where(a => a.Name.ToLower().Contains(searchText)).ToList();
        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}
