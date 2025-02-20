using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class UserDetailsPage : ContentPage
    {
        private User _user;
        private readonly ApiService _apiService = new();

        public UserDetailsPage(User user = null)
        {
            InitializeComponent();
            _user = user;
            if (user != null)
            {
                UsernameEntry.Text = user.Username;
                PasswordEntry.Text = "";
                FirstNameEntry.Text = user.FirstName;
                MiddleNameEntry.Text = user.MiddleName;
                LastNameEntry.Text = user.LastName;
                BirthDatePicker.Date = user.BirthDate;
                MobileEntry.Text = user.Mobile;
                EmailEntry.Text = user.Email;
                AddressEntry.Text = user.Address;
                NoteEditor.Text = user.Note;
            }
            LoadAndSetRole(user?.RoleId);
        }

        private async void LoadAndSetRole(int? roleId)
        {
            var roles = await _apiService.GetRolesAsync() ?? new List<Role>();
            if (!roles.Any())
            {
                await DisplayAlert("Error", "No roles found. Please check the API.", "OK");
                return;
            }

            RolePicker.ItemsSource = roles;
            RolePicker.SelectedItem = roles.FirstOrDefault(r => r.Id == roleId);
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            var user = _user ?? new User
            {
                Username = EmailEntry.Text,
                Password = PasswordEntry.Text,
                FirstName = FirstNameEntry.Text,
                MiddleName = MiddleNameEntry.Text,
                LastName = LastNameEntry.Text,
                BirthDate = BirthDatePicker.Date,
                Mobile = MobileEntry.Text,
                Email = EmailEntry.Text,
                Address = AddressEntry.Text,
                Note = NoteEditor.Text,
                RoleId = RolePicker.SelectedIndex + 1
            };

            string confirmPassword = ConfirmPasswordEntry.Text;

            // Validate user data including confirm password
            var validation = UserValidationService.ValidateUser(user, confirmPassword);
            if (!validation.IsValid)
            {
                await DisplayAlert("Validation Error", validation.ErrorMessage, "OK");
                return;
            }

            bool success = _user != null
                ? await _apiService.UpdateUserAsync(user)
                : await _apiService.RegisterUserAsync(user);

            string message = success
                ? (_user != null ? "User updated successfully!" : "User created successfully!")
                : "Failed to save user. Please try again.";

            await DisplayAlert(success ? "Success" : "Error", message, "OK");

            if (success)
                await Navigation.PushAsync(new UserListPage());
        }



    }
}
