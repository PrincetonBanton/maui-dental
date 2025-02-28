using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class DentistListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<DentistVM> _allDentists= new();

        public DentistListPage()
        {
            InitializeComponent();
            BindingContext = this;
            LoadDentistList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadDentistList();
        }
        private async void LoadDentistList()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    _allDentists = await _apiService.GetDentistsAsync() ?? new List<DentistVM>();
                }
                else
                {
                    //_allPatients = SampleData.GetSampleUsers(); //Replace w offline data sync
                }

                DentistListView.ItemsSource = _allDentists;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is DentistVM selectedDentist)
            {
                await Navigation.PushAsync(new DentistDetailsPage(selectedDentist));
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is DentistVM selectedDentist)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this dentist?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeleteDentistAsync(selectedDentist.Id);
                LoadDentistList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Dentist deleted." : "Failed to delete dentist.", "OK");
            }
        }

        private async void DentistListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is DentistVM selectedDentist)
            {
                await Navigation.PushAsync(new DentistDetailsPage(selectedDentist));
            }
            ((ListView)sender).SelectedItem = null;
        }


        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();
            DentistListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allDentists
                : _allDentists.Where(dentist => dentist.FullName.ToLower().Contains(searchText)).ToList();

        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private async void OnCreateDentistButtonClicked(object sender, EventArgs e)
        {

            await Navigation.PushAsync(new DentistDetailsPage());
        }

    }
}