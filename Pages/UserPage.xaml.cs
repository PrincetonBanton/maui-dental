using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class UserPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<User> _allUsers = new();

        public UserPage()
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
                _allUsers = await _apiService.GetUsersAsync() ?? new List<User>();
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
            string action = await DisplayActionSheet(" ", "Cancel", null, "Edit", "Delete");
            if (action == "Edit")
            {
                //string userDetails = $"ID: {selectedUser.Id}\n" +
                //     $"Username: {selectedUser.Username}\n" +
                //     $"Role ID: {selectedUser.RoleId}\n" +
                //     $"Status: {selectedUser.Status}";
                //await DisplayAlert("User Details", userDetails, "OK");
                await Navigation.PushAsync(new AccountPage(selectedUser));
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

    }
}