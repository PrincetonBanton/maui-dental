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
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is User selectedUser)
            {
                string userDetails = $"Username: {selectedUser.Username}\n" +
                      $"First Name: {selectedUser.FirstName}\n" +
                      $"Middle Name: {selectedUser.MiddleName}\n" +
                      $"Last Name: {selectedUser.LastName}\n" +
                      $"Birth Date: {selectedUser.BirthDate.ToShortDateString()}\n" +
                      $"Mobile: {selectedUser.Mobile}\n" +
                      $"Email: {selectedUser.Email}\n" +
                      $"Address: {selectedUser.Address}\n" +
                      $"Note: {selectedUser.Note}\n" +
                      $"RoleId: {selectedUser.RoleId}";

                // Display the alert with user details
                await DisplayAlert("User Details", userDetails, "OK");

                selectedUser.RoleId = 1;
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
                await Navigation.PushAsync(new DentistDetailsPage(selectedUser));
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
        private async void OnCreateDentistButtonClicked(object sender, EventArgs e)
        {
            var newDentist = new User { RoleId = 1 };
            await Navigation.PushAsync(new UserDetailsPage(newDentist));
        }

    }
}