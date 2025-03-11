using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class PatientListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<PatientVM> _allPatients = new();

        public PatientListPage()
        {
            InitializeComponent();
            LoadPatientList();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            LoadPatientList();
            if (App.Instance.UserNavigated == "userdetails") App.Instance.UserNavigated = "userlist";
            if (App.Instance.DentistNavigated == "dentistdetails") App.Instance.DentistNavigated = "dentistlist";
        }
        private async void LoadPatientList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                _allPatients = isApiAvailable
                    ? await _apiService.GetPatientsAsync() ?? new List<PatientVM>()
                    : SampleData.GetSamplePatients(); //Replace w offline data sync

                PatientListView.ItemsSource = _allPatients;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }

        private async void OnCreatePatientButtonClicked(object sender, EventArgs e)
        {
            App.Instance.PatientNavigated = "patientdetails";
            await Navigation.PushAsync(new PatientDetailsPage());
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is PatientVM selectedPatient)
            {
                App.Instance.PatientNavigated = "patientdetails";
                await Navigation.PushAsync(new PatientDetailsPage(selectedPatient));
            }
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is PatientVM selectedPatient)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this patient?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeletePatientAsync(selectedPatient.Id);
                LoadPatientList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Patient deleted." : "Failed to delete patient.", "OK");
            }
        }

        private async void PatientListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is PatientVM selectedPatient)
            {
                App.Instance.PatientNavigated = "patientdetails";
                await Navigation.PushAsync(new PatientDetailsPage(selectedPatient));
            }
            ((ListView)sender).SelectedItem = null;
        }
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();
            PatientListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allPatients
                : _allPatients.Where(patient => patient.FullName.ToLower().Contains(searchText)).ToList();

        }
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}