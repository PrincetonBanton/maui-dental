using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class UserListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<UserVM> _allUsers = new();

        public UserListPage()
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
                    _allUsers = await _apiService.GetUsersAsync() ?? new List<UserVM>();
                }
                else
                { 
                    //_allPatients = SampleData.GetSampleUsers(); //Replace w offline data sync
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
            if (sender is ImageButton button && button.BindingContext is UserVM selectedUser)
            {
                await Navigation.PushAsync(new UserDetailsPage(selectedUser));
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UserVM selectedUser)
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
            if (e.Item is UserVM selectedUser)
            {
                await Navigation.PushAsync(new UserDetailsPage(selectedUser));
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
        private async void OnCreateUserButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new UserDetailsPage());
        }

    }
}