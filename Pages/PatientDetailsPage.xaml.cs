using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages;

public partial class PatientDetailsPage : ContentPage
{
    private User _patient;
    private readonly ApiService _apiService = new();

    public PatientDetailsPage(User patient = null)
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
        MiddleNameEntry.Text = _patient.MiddleName;
        AddressEntry.Text = _patient.Address;
        BirthDatePicker.Date = _patient.BirthDate;
        EmailEntry.Text = _patient.Email;
        MobileEntry.Text = _patient.Mobile;
        NoteEditor.Text = _patient.Note;
    }
}
