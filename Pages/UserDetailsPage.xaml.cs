using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages;

public partial class UserDetailsPage : ContentPage
{
    private UserVM _user;
    private readonly ApiService _apiService = new();

    public UserDetailsPage(UserVM user = null)
    {
        InitializeComponent();
        _user = user;

        if (_user != null)
        {
            BindUserDetails();
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
}
