using DentalApp.Models;
using DentalApp.Services;
using System.Xml;

namespace DentalApp.Pages;

public partial class PatientDetailsPage : ContentPage
{
    private PatientVM _patient;
    private readonly ApiService _apiService = new();


    public PatientDetailsPage(PatientVM patient = null)
    {
        InitializeComponent();
        _patient = patient;
        if (_patient != null)
        {
            BindPatientDetails();
        }
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

        //string patientInfo = $"FirstName: {_patient.FirstName} " +
        //                     $"MiddleName: {_patient.MiddleName} " +
        //                     $"LastName: {_patient.LastName} " +
        //                     $"BirthDate: {_patient.BirthDate} " +
        //                     $"Email: {_patient.Email} " +
        //                     $"Mobile: {_patient.Mobile} " +
        //                     $"Address: {_patient.Address} " +
        //                     $"Note: {_patient.Note} " +
        //                     $"PatientNo: {_patient.PatientNo}";
        //await DisplayAlert("Patient Information", patientInfo, "OK");

        var (isValid, errorMessage) = PatientValidationService.ValidatePatient(_patient);

        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }
        bool success = _patient.Id != 0
        ? await _apiService.UpdatePatientAsync(_patient)
            : await _apiService.CreatePatientAsync(_patient);
        string message = success
            ? (_patient.Id != 0 ? "Patient updated successfully!" : "Patient created successfully!")
            : "Failed to save patient. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
            await Navigation.PopAsync();
    }
}
