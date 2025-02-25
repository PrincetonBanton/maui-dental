using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class PatientListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<User> _allUsers = new();

        public PatientListPage()
        {
            InitializeComponent();
            LoadUserList();
        }
        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadUserList();
        }
        private async void LoadUserList()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    _allUsers = await _apiService.GetPatientsAsync() ?? new List<User>();
                }
                else
                {
                    _allUsers = SampleData.GetSampleUsers(); //Replace w offline data sync
                }

                UserListView.ItemsSource = _allUsers;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is User selectedUser)
            {
                selectedUser.RoleId = 2;
                await Navigation.PushAsync(new UserDetailsPage(selectedUser));
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is User selectedUser)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this user?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeleteUserAsync(selectedUser.Id);
                LoadUserList();
                await DisplayAlert(success ? "Success" : "Error", success ? "User deleted." : "Failed to delete user.", "OK");
            }
        }

        private async void UserListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is User selectedUser)
            {
                await Navigation.PushAsync(new PatientDetailsPage(selectedUser));
            }
            ((ListView)sender).SelectedItem = null;
        }


        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();

            UserListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allUsers
                : _allUsers.Where(user => user.FullName.ToLower().Contains(searchText)).ToList();

        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private async void OnCreatePatientButtonClicked(object sender, EventArgs e)
        {
            var newPatient = new User { RoleId = 2 }; 
            await Navigation.PushAsync(new UserDetailsPage(newPatient));
        }

    }
}