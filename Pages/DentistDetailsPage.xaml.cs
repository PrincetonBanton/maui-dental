using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class DentistDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private DentistVM _dentist;
    private ObservableCollection<DentistVM> _allDentists = new();
    public DentistDetailsPage(ObservableCollection<DentistVM> allDentists, DentistVM dentist = null)
    {
        InitializeComponent();
        _allDentists = allDentists;
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
