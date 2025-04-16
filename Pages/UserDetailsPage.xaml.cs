using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class UserDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private UserVM _user;
    private ObservableCollection<UserVM> _allUsers = new();
    private bool _isEditMode;

    public UserDetailsPage(ObservableCollection<UserVM> allUsers, UserVM user = null)
    {
        InitializeComponent();
        _allUsers = allUsers;
        _user = user;
        //LoadRoles();

        if (_user != null) {
            _isEditMode = true;
            BindUserDetails();
            ToggleFieldsVisibility();
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.UserNavigated == "userlist") await Navigation.PopAsync(); 
    }
    private async void LoadRoles()
    {
        var roles = await _apiService.GetRolesAsync();
        RolePicker.ItemsSource = roles;

        if (_user != null)
        {
            var selectedRole = roles.FirstOrDefault(r => r.Id == _user.RoleId);
            RolePicker.SelectedItem = selectedRole;
        }
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
    private void ToggleFieldsVisibility()  
    {
        PasswordLabel.IsVisible = !_isEditMode;
        PasswordEntry.IsVisible = !_isEditMode;
        ConfirmPasswordLabel.IsVisible = !_isEditMode;
        ConfirmPasswordEntry.IsVisible = !_isEditMode;
        RoleLabel.IsVisible = !_isEditMode;
        RolePicker.IsVisible = !_isEditMode;
        UsernameEntry.Text = _user.Username;
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

        var (isValid, errorMessage) = UserValidationService.ValidateUser(_user, ConfirmPasswordEntry.Text, _isEditMode);    
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        var jsonUser = System.Text.Json.JsonSerializer.Serialize(_user, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await DisplayAlert("User Object", jsonUser, "OK");

        bool success = _user.Id != 0
            ? await _apiService.UpdateUserAsync(_user)
            : await _apiService.CreateUserAsync(_user);

        if (success)
        {
            var updatedList = await _apiService.GetUsersAsync() ?? new List<UserVM>();
            _allUsers.Clear();
            updatedList.ForEach(_allUsers.Add);
        }   

        string message = success
            ? (_user.Id != 0 ? "User updated successfully!" : "User created successfully!")
            : "Failed to save user. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            _user = null;
            await Navigation.PopAsync();
        }
    }
}
