using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class DentistListPage : ContentPage
    {
        private readonly DentistService _dentistService = new();
        private ObservableCollection<DentistVM> _allDentists = new();

        public DentistListPage()
        {
            InitializeComponent();
            LoadDentistList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            if (App.Instance.UserNavigated == "userdetails") App.Instance.UserNavigated = "userlist";
            if (App.Instance.PatientNavigated == "patientdetails") App.Instance.PatientNavigated = "patientlist";
        }
        private async void LoadDentistList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                var dentistList = isApiAvailable
                    ? await _dentistService.GetDentistsAsync() ?? new List<DentistVM>()
                    : SampleData.GetSampleDentists(); 

                _allDentists.Clear();
                dentistList.ForEach(_allDentists.Add);
                DentistListView.ItemsSource = _allDentists;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }
        private async void OnCreateDentistButtonClicked(object sender, EventArgs e)
        {
            App.Instance.DentistNavigated = "dentistdetails";
            await Navigation.PushAsync(new DentistDetailsPage(_allDentists));
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is DentistVM selectedDentist)
            {
                App.Instance.DentistNavigated = "dentistdetails";
                await Navigation.PushAsync(new DentistDetailsPage(_allDentists, selectedDentist));
            }
        }
        private async void DentistListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //if (e.Item is DentistVM selectedDentist)
            //{
            //    App.Instance.DentistNavigated = "dentistdetails";
            //    await Navigation.PushAsync(new DentistDetailsPage(_allDentists, selectedDentist));
            //}
            //((ListView)sender).SelectedItem = null;
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is DentistVM selectedDentist)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this dentist?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _dentistService.DeleteDentistAsync(selectedDentist.Id);
                LoadDentistList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Dentist deleted." : "Failed to delete dentist.", "OK");
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
            DentistListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allDentists
                : _allDentists.Where(dentist => dentist.FullName.ToLower().Contains(searchText)).ToList();

        }
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();

    }
}