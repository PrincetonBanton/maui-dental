using DentalApp.Models;
using DentalApp.Services;
using System.Xml;

namespace DentalApp.Pages;

public partial class DentistDetailsPage : ContentPage
{
    private DentistVM _dentist;
    private readonly ApiService _apiService = new();


    public DentistDetailsPage(DentistVM dentist = null)
    {
        InitializeComponent();
        _dentist = dentist;
        if (_dentist != null) BindPatientDetails();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.DentistNavigated == "dentistlist") await Navigation.PopAsync();
    }
    private void BindPatientDetails()
    {
        FirstNameEntry.Text = _dentist.FirstName;
        LastNameEntry.Text = _dentist.LastName;
        //MiddleNameEntry.Text = _dentist.MiddleName;
        AddressEntry.Text = _dentist.Address;
        BirthDatePicker.Date = _dentist.BirthDate;
        EmailEntry.Text = _dentist.Email;
        MobileEntry.Text = _dentist.Mobile;
        NoteEditor.Text = _dentist.Note;
    }
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _dentist ??= new DentistVM();

        _dentist.FirstName = FirstNameEntry.Text;
        _dentist.MiddleName = MiddleNameEntry.Text;
        _dentist.LastName = LastNameEntry.Text;
        _dentist.BirthDate = BirthDatePicker.Date;
        _dentist.Email = EmailEntry.Text;
        _dentist.Mobile = MobileEntry.Text;
        _dentist.Address = AddressEntry.Text;
        _dentist.Note = NoteEditor.Text;

        var (isValid, errorMessage) = DentistValidationService.ValidateDentist(_dentist);

        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }
        bool success = _dentist.Id != 0
        ? await _apiService.UpdateDentistAsync(_dentist)
            : await _apiService.CreateDentistAsync(_dentist);
        string message = success
            ? (_dentist.Id != 0 ? "Patient updated successfully!" : "Dentist created successfully!")
            : "Failed to save dentist. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
            await Navigation.PopAsync();
    }
}
