using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages;

public partial class DentistDetailsPage : ContentPage
{
    private User _dentist;
    private readonly ApiService _apiService = new();

    public DentistDetailsPage(User dentist = null)
    {
        InitializeComponent();
        _dentist = dentist;
        if (_dentist != null)
        {
            BindDentistDetails();
        }
    }

    private void BindDentistDetails()
    {
        FirstNameEntry.Text = _dentist.FirstName;
        LastNameEntry.Text = _dentist.LastName;
        MiddleNameEntry.Text = _dentist.MiddleName;
        AddressEntry.Text = _dentist.Address;
        BirthDatePicker.Date = _dentist.BirthDate;
        EmailEntry.Text = _dentist.Email;
        MobileEntry.Text = _dentist.Mobile;
        NoteEditor.Text = _dentist.Note;
    }
}
