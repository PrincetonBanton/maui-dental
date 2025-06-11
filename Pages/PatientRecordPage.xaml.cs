using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;
using System.Xml;

namespace DentalApp.Pages;

public partial class PatientRecordPage : ContentPage
{
    private readonly PaymentService _paymentService = new();
    private readonly SaleService _saleService = new();
    private readonly AppointmentService _appointmentService = new();
    private PatientVM _patient;
    private ObservableCollection<PatientVM> _allPatients = new();
    private ObservableCollection<Appointment> _allAppointments = new();
    private ObservableCollection<Payment> _allPayments = new();

    public PatientRecordPage(ObservableCollection<PatientVM> allPatients, PatientVM patient = null)
    {
        InitializeComponent();
        _allPatients = allPatients;
        _patient = patient;
        BindPatientDetails();
        LoadAppointmentsForPatient(_patient.Id);
        LoadPaymentsForPatient(_patient.Id);
    }
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (App.Instance.PatientNavigated == "patientlist") await Navigation.PopAsync();
    }
    private void BindPatientDetails()
    {
        //NameLabel.Text = $"Name: {_patient.FirstName} {_patient.MiddleName} {_patient.LastName}".Trim();
        //BdayLabel.Text = _patient.BirthDate.ToString();
        NameLabel.Text = $"{_patient.FirstName} {_patient.MiddleName} {_patient.LastName}".Trim();
        BdayLabel.Text = $"Age: {CalculateAge(_patient.BirthDate).ToString()}";
        MobileLabel.Text = $"Mobile: {_patient.Mobile}";
        EmailLabel.Text = $"Email: {_patient.Email}"; ;   
    }

    private int CalculateAge(DateTime birthDate)
    {
        int birthYear = birthDate.Year;
        int currentYear = DateTime.Today.Year;
        return currentYear - birthYear;
    }
private async void LoadAppointmentsForPatient(int patientId)
{
    await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
    bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;

    try
    {
        var appointments = isApiAvailable
            ? await _appointmentService.GetAppointmentsAsync() ?? new List<Appointment>()
            : SampleData.GetSampleAppointments();

        var filtered = appointments?.Where(a => a.PatientId == patientId) ?? Enumerable.Empty<Appointment>();

        _allAppointments = new ObservableCollection<Appointment>(filtered);            
        AppointmentListView.ItemsSource = _allAppointments;
    }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to load appointments. {ex.Message}", "OK");
    }
}
private async void LoadPaymentsForPatient(int patientId)
{
    await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
    bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;

    try
    {
        var payments = isApiAvailable
            ? await _paymentService.GetPaymentsAsync(patientId) ?? new List<Payment>()
            : new List<Payment>();

        //var jsonUser = System.Text.Json.JsonSerializer.Serialize(payments, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        //await DisplayAlert("User Object", jsonUser, "OK");

        foreach (var payment in payments)
        {
            var saleDetail = await _saleService.GetSaleDetailAsync(payment.SaleId);
            payment.SaleProductName = saleDetail?.Items.FirstOrDefault()?.ProductName ?? "Unknown Product";
        }

            PaymentListView.ItemsSource = payments;
            decimal total = payments.Sum(p => p.PaymentAmount);
            PaymentTotalLabel.Text = total.ToString("N2"); 
        }
    catch (Exception ex)
    {
        await DisplayAlert("Error", $"Failed to load payments. {ex.Message}", "OK");
    }
}
}
