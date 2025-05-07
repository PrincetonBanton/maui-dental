using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;
using Windows.System;

namespace DentalApp.Pages;

public partial class DentistDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private DentistVM _dentist;
    private ObservableCollection<DentistVM> _allDentists = new();
    private bool _isEditMode;

    public DentistDetailsPage(ObservableCollection<DentistVM> allDentists, DentistVM dentist = null)
    {
        InitializeComponent();
        _allDentists = allDentists;
        _dentist = dentist;

        if (_dentist != null)
        {
            _isEditMode = true;
            BindPatientDetails();
            ToggleFieldsVisibility();
        }
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.DentistNavigated == "dentistlist") await Navigation.PopAsync();
    }
    private void BindPatientDetails()
    {
        //var jsonUser = System.Text.Json.JsonSerializer.Serialize(_dentist, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        //DisplayAlert("User Object", jsonUser, "OK");

        FirstNameEntry.Text = _dentist.FirstName;
        LastNameEntry.Text = _dentist.LastName;
        MiddleNameEntry.Text = _dentist.MiddleName;
        AddressEntry.Text = _dentist.Address;
        BirthDatePicker.Date = _dentist.BirthDate;
        EmailEntry.Text = _dentist.Email;
        MobileEntry.Text = _dentist.Mobile;
        NoteEditor.Text = _dentist.Note;
    }
    private void ToggleFieldsVisibility()
    {
        UsernameEntry.Text = _dentist.Username;
        LoginInfoFrame.IsVisible = !_isEditMode;
    }
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _dentist ??= new DentistVM();
        _dentist.Username = UsernameEntry.Text;
        _dentist.Password = PasswordEntry.Text;
        _dentist.FirstName = FirstNameEntry.Text;
        _dentist.MiddleName = MiddleNameEntry.Text;
        _dentist.LastName = LastNameEntry.Text;
        _dentist.BirthDate = BirthDatePicker.Date;
        _dentist.Email = EmailEntry.Text;
        _dentist.Mobile = MobileEntry.Text;
        _dentist.Address = AddressEntry.Text;
        _dentist.Note = NoteEditor.Text;

        var (isValid, errorMessage) = DentistValidationService.ValidateDentist(_dentist, ConfirmPasswordEntry.Text, _isEditMode);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        bool success = _dentist.Id != 0
            ? await _apiService.UpdateDentistAsync(_dentist)
            : await _apiService.CreateDentistAsync(_dentist);

            if (success)
            {
                var updatedList = await _apiService.GetDentistsAsync() ?? new List<DentistVM>();
                _allDentists.Clear();
                updatedList.ForEach(_allDentists.Add);
            }

        string message = success
            ? (_dentist.Id != 0 ? "Patient updated successfully!" : "Dentist created successfully!")
            : "Failed to save dentist. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            _dentist = null;
            await Navigation.PopAsync();
        }
    }

}
