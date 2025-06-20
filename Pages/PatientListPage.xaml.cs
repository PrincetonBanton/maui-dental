using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class PatientListPage : ContentPage
    {
        private readonly PatientService _patientService = new();
        private ObservableCollection<PatientVM> _allPatients = new();

        public PatientListPage()
        {
            InitializeComponent();
            LoadPatientList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (App.Instance.UserNavigated == "userdetails") App.Instance.UserNavigated = "userlist";
            if (App.Instance.DentistNavigated == "dentistdetails") App.Instance.DentistNavigated = "dentistlist";
        }
        private async void LoadPatientList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                var patientList = isApiAvailable
                    ? await _patientService.GetPatientsAsync() ?? new List<PatientVM>()
                    : SampleData.GetSamplePatients(); 
                _allPatients.Clear();
                patientList.ForEach(_allPatients.Add);
                PatientListView.ItemsSource = _allPatients;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }   
        private async void OnCreatePatientButtonClicked(object sender, EventArgs e)
        {
            App.Instance.PatientNavigated = "patientdetails";
            await Navigation.PushAsync(new PatientDetailsPage(_allPatients));
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is PatientVM selectedPatient)
            {
                App.Instance.PatientNavigated = "patientdetails";
                await Navigation.PushAsync(new PatientDetailsPage(_allPatients, selectedPatient));

            }
        }
        private async void PatientListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is PatientVM selectedPatient)
            {
                App.Instance.PatientNavigated = "patientdetails";
                //await Navigation.PushAsync(new PatientDetailsPage(_allPatients, selectedPatient));
                await Navigation.PushAsync(new PatientRecordPage(_allPatients, selectedPatient));
            }
            ((ListView)sender).SelectedItem = null;
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is PatientVM selectedPatient)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this patient?", "Yes", "No");
                if (!confirmDelete) return;
                var success = await _patientService.DeletePatientAsync(selectedPatient.Id);
                LoadPatientList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Patient deleted." : "Failed to delete patient.", "OK");
            }
        }
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            if (!ApiConnectivityService.Instance.IsApiAvailable)
            {
                DisplayAlert("Offline", "You are offline. Search is not available.", "OK");
                SearchBar.Text = string.Empty;
                return;
            }
            var searchText = e.NewTextValue.ToLower();
            PatientListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allPatients
                : _allPatients.Where(patient => patient.FullName.ToLower().Contains(searchText)).ToList();

        }
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}