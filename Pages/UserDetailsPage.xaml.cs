using DentalApp.Models;
using DentalApp.Models.Enum;
using DentalApp.Services;

namespace DentalApp.Pages;

public partial class UserDetailsPage : ContentPage
{
    private UserVM _user;
    private readonly ApiService _apiService = new();

    public UserDetailsPage(UserVM user = null)
    {
        InitializeComponent();
        _user = user;


        if (_user != null)
        {
            BindUserDetails();
        }
        LoadRoles();
    }
    private async void LoadRoles()
    {
        RolePicker.ItemsSource = await _apiService.GetRolesAsync();
    }
    private void BindUserDetails()
    {
        FirstNameEntry.Text = _user.FirstName;
        LastNameEntry.Text = _user.LastName;
        MiddleNameEntry.Text = _user.MiddleName;
        AddressEntry.Text = _user.Address;
        BirthDatePicker.Date = _user.BirthDate;
        EmailEntry.Text = _user.Email;
        MobileEntry.Text = _user.Mobile;
        NoteEditor.Text = _user.Note;
        
    }
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _user ??= new UserVM();

        _user.Username = UsernameEntry.Text;
        _user.Password = PasswordEntry.Text;
        _user.FirstName = FirstNameEntry.Text;
        _user.MiddleName = MiddleNameEntry.Text;
        _user.LastName = LastNameEntry.Text;
        _user.Address = AddressEntry.Text;
        _user.BirthDate = BirthDatePicker.Date;
        _user.Email = EmailEntry.Text;
        _user.Mobile = MobileEntry.Text;
        _user.Note = NoteEditor.Text;

        // Ensure RolePicker has a valid selection
        if (RolePicker.SelectedItem is Role selectedRole)
        {
            _user.RoleId = selectedRole.Id;
        }
        else
        {
            _user.RoleId = 0; // Default or invalid role
        }


        string userInfo = $"Username: {_user.Username} " +
                          $"Password: {_user.Password} " +
                          $"FirstName: {_user.FirstName} " +
                          $"MiddleName: {_user.MiddleName} " +
                          $"LastName: {_user.LastName} " +
                          $"BirthDate: {_user.BirthDate:d} " +
                          $"Email: {_user.Email} " +
                          $"Mobile: {_user.Mobile} " +
                          $"Address: {_user.Address} " +
                          $"Note: {_user.Note} " +
                          $"Role: {(_user.RoleId > 0 ? _user.RoleId.ToString() : "Not Assigned")}";

        await DisplayAlert("User Information", userInfo, "OK");

        // Use the correct validation service
        var (isValid, errorMessage) = UserValidationService.ValidateUser(_user, ConfirmPasswordEntry.Text);


        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        bool success = _user.Id != 0
        ? await _apiService.UpdateUserAsync(_user)
            : await _apiService.CreateUserAsync(_user);
        string message = success
            ? (_user.Id != 0 ? "Patient updated successfully!" : "User created successfully!")
            : "Failed to save user. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
            await Navigation.PopAsync();
    }
}
