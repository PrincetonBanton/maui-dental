using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages;

public partial class StaffDetailsPage : ContentPage
{
    private User _staff;
    private readonly ApiService _apiService = new();

    public StaffDetailsPage(User staff = null)
    {
        InitializeComponent();
        _staff = staff;

        if (_staff != null)
        {
            BindStaffDetails();
        }
    }

    private void BindStaffDetails()
    {
        FirstNameEntry.Text = _staff.FirstName;
        LastNameEntry.Text = _staff.LastName;
        MiddleNameEntry.Text = _staff.MiddleName;
        AddressEntry.Text = _staff.Address;
        BirthDatePicker.Date = _staff.BirthDate;
        EmailEntry.Text = _staff.Email;
        MobileEntry.Text = _staff.Mobile;
        NoteEditor.Text = _staff.Note;
    }
}
