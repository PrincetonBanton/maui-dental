using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;

namespace DentalApp.Pages;

public partial class UserDetailsPage : ContentPage
{
    private UserVM _user;
    private readonly ApiService _apiService = new();
    private bool _isEditMode;

    public UserDetailsPage(UserVM user = null)
    {
        InitializeComponent();
        _user = user;
        _isEditMode = _user != null;

        if (_isEditMode) BindUserDetails();
        LoadRoles();
        ToggleFieldsVisibility();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.UserNavigated == "userlist") await Navigation.PopAsync(); 
    }
    private async void LoadRoles() => RolePicker.ItemsSource = await _apiService.GetRolesAsync();
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
    private void ToggleFieldsVisibility()  //Replace this with proper standard
    {
        PasswordLabel.IsVisible = !_isEditMode;
        PasswordEntry.IsVisible = !_isEditMode;
        ConfirmPasswordLabel.IsVisible = !_isEditMode;
        ConfirmPasswordEntry.IsVisible = !_isEditMode;
        RoleLabel.IsVisible = !_isEditMode;
        RolePicker.IsVisible = !_isEditMode;
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
        _user.RoleId = RolePicker.SelectedItem is Role selectedRole ? selectedRole.Id : 0;

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
            ? (_user.Id != 0 ? "User updated successfully!" : "User created successfully!")
            : "Failed to save user. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success) await Navigation.PopAsync();
    }
}
