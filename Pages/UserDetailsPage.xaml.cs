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
            _user ??= new User();

            if (RolePicker.SelectedItem is Role selectedRole && selectedRole.Id == 2)
            {
                // Auto-fill login information for patients
                _user.Username = $"{FirstNameEntry.Text}{LastNameEntry.Text}";
                _user.Password = "123456";
                ConfirmPasswordEntry.Text = _user.Password;
            }
            else
            {
                _user.Username = EmailEntry.Text;
                _user.Password = PasswordEntry.Text;
            }

            _user.FirstName = FirstNameEntry.Text;
            _user.MiddleName = MiddleNameEntry.Text;
            _user.LastName = LastNameEntry.Text;
            _user.BirthDate = BirthDatePicker.Date;
            _user.Email = EmailEntry.Text;
            _user.Mobile = MobileEntry.Text;
            _user.Address = AddressEntry.Text;
            _user.Note = NoteEditor.Text;
            _user.RoleId = RolePicker.SelectedIndex + 1;

            string confirmPassword = ConfirmPasswordEntry.Text;

            // Validate user data including confirm password
            var validation = UserValidationService.ValidateUser(_user, confirmPassword);
            if (!validation.IsValid)
            {
                await DisplayAlert("Validation Error", validation.ErrorMessage, "OK");
                return;
            }

            // Save or update user
            bool success = _user.Id != 0
                ? await _apiService.UpdateUserAsync(_user)
                : await _apiService.RegisterUserAsync(_user);

            string message = success
                ? (_user.Id != 0 ? "User updated successfully!" : "User created successfully!")
                : "Failed to save user. Please try again.";

            await DisplayAlert(success ? "Success" : "Error", message, "OK");

            if (success)
                await Navigation.PopAsync();
        }
        private void RolePicker_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (RolePicker.SelectedItem is Role selectedRole)
            {
                // Assuming RoleId 2 corresponds to "Patient"
                LoginFrame.IsVisible = selectedRole.Id != 2;
            }
        }
    }
}
