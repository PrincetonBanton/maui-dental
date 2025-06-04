using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class AppointmentDetailsPage : ContentPage
{
    private readonly PatientService _patientService = new();
    private readonly DentistService _dentistService = new();
    private readonly AppointmentService _appointmentService = new();
    private Appointment _appointment;
    private ObservableCollection<Appointment> _allAppointments;
    private List<PatientVM> _patients;

    public AppointmentDetailsPage(ObservableCollection<Appointment> allAppointments, Appointment appointment = null)
    {
        InitializeComponent();
        _allAppointments = allAppointments;
        _appointment = appointment;
        PopulateTimePickers();
        if (appointment != null)
        {
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
            var patients = await _patientService.GetPatientsAsync();
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
            var dentists = await _dentistService.GetDentistsAsync();
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
        _appointment ??= new Appointment();
        _appointment.Title = TitleEntry.Text;
        _appointment.Description = DescriptionEditor.Text;
        _appointment.StartDate = DatePicker.Date + DateTime.Parse(StartTimePicker.SelectedItem.ToString()).TimeOfDay;
        _appointment.EndDate = DatePicker.Date + DateTime.Parse(EndTimePicker.SelectedItem.ToString()).TimeOfDay;
        _appointment.PatientId = PatientPicker.SelectedItem is PatientVM selectedPatient ? selectedPatient.Id : 0;
        _appointment.DentistId = DentistPicker.SelectedItem is DentistVM selectedDentist ? selectedDentist.Id : 0;

        var (isValid, errorMessage) = AppointmentValidationService.ValidateAppointment(_appointment);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        // Conflict check
        var existingAppointments = await _appointmentService.GetAppointmentsAsync();
        if (AppointmentValidationService.HasTimeConflict(_appointment, existingAppointments))
        {
            await DisplayAlert("Conflict", "This appointment overlaps with another appointment for the selected dentist.", "OK");
            return;
        }

        //var jsonUser = System.Text.Json.JsonSerializer.Serialize(_appointment, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        //await DisplayAlert("User Object", jsonUser, "OK");

        bool success = _appointment.Id != 0
            ? await _appointmentService.UpdateAppointmentAsync(_appointment)
            : await _appointmentService.CreateAppointmentAsync(_appointment);

        if (success)
        {
            var updatedList = await _appointmentService.GetAppointmentsAsync() ?? new List<Appointment>();
            _allAppointments.Clear();
            updatedList.ForEach(_allAppointments.Add);
        }

        string message = success
            ? (_appointment.Id != 0 ? "Appointment updated successfully!" : "Appointment created successfully!")
            : "Failed to save dentist. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            _appointment = null;
            await Navigation.PopAsync();
        }
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
        // Set default selections
        StartTimePicker.SelectedIndex = times.IndexOf("08:00 AM");
        EndTimePicker.SelectedIndex = times.IndexOf("08:30 AM");
    }
    private void StartTimePicker_SelectedIndexChanged(object sender, EventArgs e)
    {
        if (StartTimePicker.SelectedItem is string selectedTimeStr &&
            DateTime.TryParse(selectedTimeStr, out DateTime selectedStartTime))
        {
            var newEndTime = selectedStartTime.AddHours(.5);
            string formattedNewEndTime = newEndTime.ToString("hh:mm tt");
            var index = EndTimePicker.ItemsSource.IndexOf(formattedNewEndTime);
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