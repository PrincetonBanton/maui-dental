using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class PatientDetailsPage : ContentPage
{
    private readonly PatientService _patientService = new();
    private PatientVM _patient;
    private ObservableCollection<PatientVM> _allPatients = new();

    public PatientDetailsPage(ObservableCollection<PatientVM> allPatients, PatientVM patient = null)
    {
        InitializeComponent();
        _allPatients = allPatients;
        _patient = patient;
        if (_patient != null) BindPatientDetails();
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.PatientNavigated == "patientlist") await Navigation.PopAsync();
    }
    private void BindPatientDetails()
    {
        FirstNameEntry.Text = _patient.FirstName;
        LastNameEntry.Text = _patient.LastName;
        //MiddleNameEntry.Text = _patient.MiddleName;
        AddressEntry.Text = _patient.Address;
        BirthDatePicker.Date = _patient.BirthDate;
        EmailEntry.Text = _patient.Email;
        MobileEntry.Text = _patient.Mobile;
        NoteEditor.Text = _patient.Note;
        //PatientNoEntry.Text = _patient.PatientNo;
    }
    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _patient ??= new PatientVM();
        _patient.FirstName = FirstNameEntry.Text;
        _patient.MiddleName = MiddleNameEntry.Text;
        _patient.LastName = LastNameEntry.Text;
        _patient.BirthDate = BirthDatePicker.Date;
        _patient.Email = EmailEntry.Text;
        _patient.Mobile = MobileEntry.Text;
        _patient.Address = AddressEntry.Text;
        _patient.Note = NoteEditor.Text;
        _patient.PatientNo = PatientNoEntry.Text;

        var (isValid, errorMessage) = PatientValidationService.ValidatePatient(_patient);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }
        bool success = _patient.Id != 0
            ? await _patientService.UpdatePatientAsync(_patient)
            : await _patientService.CreatePatientAsync(_patient);

        if (success)
        {
            var updatedList = await _patientService.GetPatientsAsync() ?? new List<PatientVM>();
            _allPatients.Clear();
            updatedList.ForEach(_allPatients.Add);
        }

        string message = success
            ? (_patient.Id != 0 ? "Patient updated successfully!" : "Patient created successfully!")
            : "Failed to save patient. Please try again.";
        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            _patient = null;
            await Navigation.PopAsync();
        }
    }
}
