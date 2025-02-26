using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class UserListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<User> _allUsers = new();

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
                    _allUsers = await _apiService.GetUsersAsync() ?? new List<User>();
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
            if (e.Item is User selectedUser)
            {
                Page detailPage = selectedUser.RoleId switch
                {
                    1 => new DentistDetailsPage(selectedUser),
                    2 => new PatientDetailsPage(selectedUser),
                    3 => new StaffDetailsPage(selectedUser),
                    _ => null
                };

                await (detailPage != null
                  ? Navigation.PushAsync(detailPage)
                  : DisplayAlert("Error", "Unknown role. Cannot navigate to detail page.", "OK"));
            }

            ((ListView)sender).SelectedItem = null; // Deselect the item
        }


        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();

            UserListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allUsers
                : _allUsers.Where(user => user.FullName.ToLower().Contains(searchText)).ToList();

        }
        private void OnCategoryPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var selectedRole = CategoryPicker.SelectedItem as string;
            UserListView.ItemsSource = string.IsNullOrEmpty(selectedRole) || selectedRole == "All"
                ? _allUsers
                : _allUsers.Where(user => user.RoleName.Equals(selectedRole, StringComparison.OrdinalIgnoreCase)).ToList();

        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private void OnDropListImageTapped(object sender, TappedEventArgs e) => CategoryPicker.Focus();
        private async void OnCreateCustomerButtonClicked(object sender, EventArgs e) => await Navigation.PushAsync(new UserDetailsPage());
    }
}