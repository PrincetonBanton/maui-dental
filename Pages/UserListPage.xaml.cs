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
                _allUsers = isApiAvailable
                    ? await _apiService.GetUsersAsync() ?? new List<UserVM>()
                    : SampleData.GetSampleUsers();
                UserListView.ItemsSource = _allUsers;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load users. Please try again.", "OK");
            }
        }
        private async void OnCreateUserButtonClicked(object sender, EventArgs e)
        {
            App.Instance.UserNavigated = "userdetails";
            await Navigation.PushAsync(new UserDetailsPage());
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is UserVM selectedUser)
            {
                var user = await _apiService.GetUserByIdAsync(selectedUser.Id);

                if (user != null)
                {
                    App.Instance.UserNavigated = "userdetails";
                    await Navigation.PushAsync(new UserDetailsPage(user));
                }
                else
                {
                    await DisplayAlert("Error", "Failed to fetch user details.", "OK");
                }
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
                App.Instance.UserNavigated = "userdetails";
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
    }
}