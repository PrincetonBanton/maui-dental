using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class DentistListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<User> _allUsers = new();

        public DentistListPage()
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
                    _allUsers = await _apiService.GetDentistsAsync() ?? new List<User>();
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

        private async void UserListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not User selectedUser) return;
            string action = await DisplayActionSheet("What you wanna do? ", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                await Navigation.PushAsync(new UserDetailsPage(selectedUser));
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this user?", "Yes", "No"))
            {
                var success = await _apiService.DeleteUserAsync(selectedUser.Id);
                LoadUserList();
                await DisplayAlert(success ? "Success" : "Error", success ? "User deleted." : "Failed to delete user.", "OK");
            }
            ((ListView)sender).SelectedItem = null;                         // Deselect the item
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();

            UserListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allUsers
                : _allUsers.Where(user => user.FullName.ToLower().Contains(searchText)).ToList();

        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private async void OnCreateDentistButtonClicked(object sender, EventArgs e) => await Navigation.PushAsync(new UserDetailsPage());

    }
}