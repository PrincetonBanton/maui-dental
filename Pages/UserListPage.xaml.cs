using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class UserListPage : ContentPage
    {
        private readonly UserService _userService = new();
        private ObservableCollection<UserVM> _allUsers = new();

        public UserListPage()
        {
            InitializeComponent();
            LoadUserList();
        }
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (App.Instance.PatientNavigated == "patientdetails") App.Instance.PatientNavigated = "patientlist";
            if (App.Instance.DentistNavigated == "dentistdetails") App.Instance.DentistNavigated = "dentistlist";
        }
        private async void LoadUserList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                var userList = isApiAvailable
                    ? await _userService.GetUsersAsync() ?? new List<UserVM>()
                    : SampleData.GetSampleUsers();
                _allUsers.Clear();
                userList.ForEach(_allUsers.Add);
                UserListView.ItemsSource = _allUsers;
            }
            catch (Exception)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }
        private async void OnCreateUserButtonClicked(object sender, EventArgs e)
        {
            App.Instance.UserNavigated = "userdetails";
            await Navigation.PushAsync(new UserDetailsPage(_allUsers));
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UserVM selectedUser)
            {
                App.Instance.UserNavigated = "userdetails";
                await Navigation.PushAsync(new UserDetailsPage(_allUsers, selectedUser));
            }
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UserVM selectedUser)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this user?", "Yes", "No");
                if (!confirmDelete) return;
                var success = await _userService.DeleteUserAsync(selectedUser.Id);
                LoadUserList();
                await DisplayAlert(success ? "Success" : "Error", success ? "User deleted." : "Failed to delete user.", "OK");
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
            UserListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allUsers
                : _allUsers.Where(user => user.FullName.ToLower().Contains(searchText)).ToList();

        }
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}