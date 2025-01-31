using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class AccountPage : ContentPage
    {
        private User _user;
        private readonly ApiService _apiService = new();

        public AccountPage(User user = null)
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
                LoadAndSetRole(user.RoleId); // Load role based on user data
            }
            else
            {
                LoadAndSetRole(null); // Load roles for new user
            }
        }

        private async void LoadAndSetRole(int? roleId)
        {
            var roles = await _apiService.GetRolesAsync() ?? new List<Role>();

            if (roles.Any())
            {
                RolePicker.ItemsSource = roles;

                if (roleId.HasValue)
                {
                    var selectedRole = roles.FirstOrDefault(r => r.Id == roleId.Value);
                    RolePicker.SelectedItem = selectedRole;
                }
            }
            else
            {
                await DisplayAlert("Error", "No roles found. Please check the API.", "OK");
            }
        }



        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            if (_user != null) 
            {
                _user.Username = UsernameEntry.Text;
                _user.Password = PasswordEntry.Text;
                _user.FirstName = FirstNameEntry.Text;
                _user.MiddleName = MiddleNameEntry.Text;
                _user.LastName = LastNameEntry.Text;
                _user.BirthDate = BirthDatePicker.Date;
                _user.Mobile = MobileEntry.Text;
                _user.Email = EmailEntry.Text;
                _user.Address = AddressEntry.Text;
                _user.Note = NoteEditor.Text;
                _user.RoleId = RolePicker.SelectedIndex + 1; // Assuming RoleId starts from 1

                var isUpdated = await _apiService.UpdateUserAsync(_user);

                if (isUpdated)
                {
                    await DisplayAlert("Success", "User updated successfully!", "OK");
                    await Navigation.PopAsync(); 
                }
                else
                {
                    await DisplayAlert("Error", "Failed to update user. Please try again.", "OK");
                }
            }
            else 
            {
                var newUser = new User
                {
                    Username = UsernameEntry.Text,
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
                var isCreated = await _apiService.RegisterUserAsync(newUser);

                if (isCreated)
                {
                    await DisplayAlert("Success", "User created successfully!", "OK");                  
                    await Navigation.PushAsync(new UserPage());
                }
                else
                {
                    await DisplayAlert("Error", "Failed to create user. Please try again.", "OK");
                }
            }
        }
    }
}
