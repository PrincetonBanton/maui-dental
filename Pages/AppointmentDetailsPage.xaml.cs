using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class AppointmentDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private Appointment _appointment;
    private ObservableCollection<Appointment> _allAppointments;

    public AppointmentDetailsPage(ObservableCollection<Appointment> allAppointments, Appointment appointment = null)
    {
        InitializeComponent();
        _allAppointments = allAppointments;
        _appointment = appointment;
        PopulateTimePickers();
        if (appointment != null)
        {
            //var jsonUser = System.Text.Json.JsonSerializer.Serialize(_appointment, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            //DisplayAlert("User Object", jsonUser, "OK");
            BindApppointmentDetails();
        }
        LoadPatients();
        LoadDentists();
    }
    private void BindApppointmentDetails()
    {
        TitleEntry.Text = _appointment.Title;
        DescriptionEditor.Text = _appointment.Description;
        DatePicker.Date = _appointment.StartDate.Date;
        StartTimePicker.SelectedIndex = StartTimePicker.ItemsSource.IndexOf(_appointment.StartDate.ToString("hh:mm tt"));
        EndTimePicker.SelectedIndex = EndTimePicker.ItemsSource.IndexOf(_appointment.EndDate.ToString("hh:mm tt"));

    }

    private async Task LoadPatients()
    {
        try
        {
            var patients = await _apiService.GetPatientsAsync();
            PatientPicker.ItemsSource = patients;

            if (_appointment != null)
            {
                var selectedPatient = patients.FirstOrDefault(p => p.Id == _appointment.PatientId);
                if (selectedPatient != null)
                    PatientPicker.SelectedItem = selectedPatient;
            }
        }
        catch
        {
            await DisplayAlert("Error", "Failed to load patients.", "OK");
        }
    }

    private async Task LoadDentists()
    {
        try
        {
            var dentists = await _apiService.GetDentistsAsync();
            DentistPicker.ItemsSource = dentists;

            if (_appointment != null)
            {
                var selectedDentist = dentists.FirstOrDefault(d => d.Id == _appointment.DentistId);
                if (selectedDentist != null)
                    DentistPicker.SelectedItem = selectedDentist;
            }
        }
        catch
        {
            await DisplayAlert("Error", "Failed to load dentists.", "OK");
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        //_dentist ??= new DentistVM();
        //_dentist.FirstName = FirstNameEntry.Text;
        //_dentist.MiddleName = MiddleNameEntry.Text;
        //_dentist.LastName = LastNameEntry.Text;
        //_dentist.BirthDate = BirthDatePicker.Date;
        //_dentist.Email = EmailEntry.Text;
        //_dentist.Mobile = MobileEntry.Text;
        //_dentist.Address = AddressEntry.Text;
        //_dentist.Note = NoteEditor.Text;

        //var (isValid, errorMessage) = DentistValidationService.ValidateDentist(_dentist);
        //if (!isValid)
        //{
        //    await DisplayAlert("Validation Error", errorMessage, "OK");
        //    return;
        //}

        //bool success = _dentist.Id != 0
        //    ? await _apiService.UpdateDentistAsync(_dentist)
        //    : await _apiService.CreateDentistAsync(_dentist);

        //if (success)
        //{
        //    var updatedList = await _apiService.GetDentistsAsync() ?? new List<DentistVM>();
        //    _allDentists.Clear();
        //    updatedList.ForEach(_allDentists.Add);
        //}

        //string message = success
        //    ? (_dentist.Id != 0 ? "Patient updated successfully!" : "Dentist created successfully!")
        //    : "Failed to save dentist. Please try again.";

        //await DisplayAlert(success ? "Success" : "Error", message, "OK");

        //if (success)
        //{
        //    _dentist = null;
        //    await Navigation.PopAsync();
        //}
    }
    private void PopulateTimePickers()
    {
        var startTime = new TimeSpan(8, 0, 0); // 8:00 AM
        var endTime = new TimeSpan(17, 0, 0);  // 5:00 PM

        var times = new List<string>();
        while (startTime <= endTime)
        {
            times.Add(DateTime.Today.Add(startTime).ToString("hh:mm tt"));
            startTime = startTime.Add(TimeSpan.FromMinutes(30));
        }

        StartTimePicker.ItemsSource = times;
        EndTimePicker.ItemsSource = times;
    }
    private void StartTimePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StartTimePicker.SelectedItem is string selectedTimeStr &&
            DateTime.TryParse(selectedTimeStr, out DateTime selectedStartTime))
        {
            var newEndTime = selectedStartTime.AddHours(.5);
            string formattedNewEndTime = newEndTime.ToString("hh:mm tt");
            var index = EndTimePicker.ItemsSource.IndexOf(formattedNewEndTime);

            // If found, select it
            EndTimePicker.SelectedIndex = index >= 0 ? index : -1;
        }
    }
    private void EndTimePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        var startText = StartTimePicker.SelectedItem as string;
        var endText = EndTimePicker.SelectedItem as string;

        if (DateTime.TryParse(startText, out var startTime) &&
            DateTime.TryParse(endText, out var endTime))
        {
            if (endTime <= startTime)
            {
                DisplayAlert("Invalid Time", "End time must be later than start time.", "OK");
                EndTimePicker.SelectedIndex = -1; // Clear invalid selection
            }
        }
    }


}