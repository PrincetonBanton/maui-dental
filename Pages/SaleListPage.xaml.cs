using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SaleListPage : ContentPage
{
    private readonly SaleService _saleService = new();
    private readonly PaymentService _paymentService = new();
    private ObservableCollection<SaleVM> _allSales = new();
    private ObservableCollection<SaleVM> _filteredSales = new();

    public SaleListPage()
    {
        InitializeComponent();
        LoadSaleList();
    }
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadSaleList();
    }
    private async void LoadSaleList()
    {
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
        bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
        try
        {
            var saleList = isApiAvailable
                   ? await _saleService.GetSalesAsync() ?? new List<SaleVM>()
                   : SampleData.GetSampleSales();
            _allSales.Clear();
            _filteredSales.Clear();
            saleList.ForEach(e =>
            {
                _allSales.Add(e);
                _filteredSales.Add(e);
            });
            SaleListView.ItemsSource = _allSales;
            UpdateSaleCount();
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to load sales. Please try again.", "OK");
        }
    }
    private async void OnCreateSaleButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SaleDetailsPage(_allSales));
    }
    private async void OnPayButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            string totalAmountString = selectedSale.TotalDue.ToString("0.00");

            string paymentInput = await DisplayPromptAsync("Payment", "Amount:", "OK", "Cancel",
                keyboard: Keyboard.Numeric, initialValue: totalAmountString);

            if (!decimal.TryParse(paymentInput, out decimal paymentAmount))
            {
                await DisplayAlert("Error", "Invalid payment amount entered.", "OK");
                return;
            }

            var payment = new Payment
            {
                SaleId = selectedSale.SaleId,
                PaymentAmount = paymentAmount,
                AmountTendered = paymentAmount, // or ask separately if needed
                PaymentType = 0, // Hardcoded as cash
                EnteredBy = 41,
                PaymentDate = DateTime.Now
            };


            bool success = await _paymentService.AddPaymentAsync(payment);

            await DisplayAlert(success ? "Success" : "Error",
                success ? $"Payment of {paymentAmount:C} recorded." : "Payment failed. Try again.",
                "OK");
            LoadSaleList();
        }
    }

    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            await Navigation.PushAsync(new SaleDetailsPage(_allSales, selectedSale));
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            bool confirmDelete = await DisplayAlert("Confirm", "Delete this sale?", "Yes", "No");
            if (!confirmDelete) return;
            var success = await _saleService.DeleteSaleAsync(selectedSale.SaleId);
            LoadSaleList();
            await DisplayAlert(success ? "Success" : "Error", success ? "Sale deleted." : "Failed to delete sale.", "OK");
        }
    }
    //---- F I L T E R
    private void OnCustomDateCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = customDateCheckBox.IsChecked;
        saleStartPicker.IsEnabled = isChecked;
        saleEndPicker.IsEnabled = isChecked;
        quickFilterCheckBox.IsChecked = !isChecked;
        todayRadioButton.IsEnabled = !isChecked;
        thisWeekRadioButton.IsEnabled = !isChecked;
        thisMonthRadioButton.IsEnabled = !isChecked;
        thisYearRadioButton.IsEnabled = !isChecked;
        if (isChecked) ApplyCustomDateFilter();
    }
    private void OnQuickFilterCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = quickFilterCheckBox.IsChecked;
        todayRadioButton.IsEnabled = isChecked;
        thisWeekRadioButton.IsEnabled = isChecked;
        thisMonthRadioButton.IsEnabled = isChecked;
        thisYearRadioButton.IsEnabled = isChecked;
        customDateCheckBox.IsChecked = !isChecked;
        saleStartPicker.IsEnabled = !isChecked;
        saleEndPicker.IsEnabled = !isChecked;
    }
    private void OnQuickFilterRadioButtonChanged(object sender, CheckedChangedEventArgs e)
    {
        if (!quickFilterCheckBox.IsChecked) return;

        DateTime today = DateTime.Today;
        DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
        DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
        int currentYear = today.Year;

        if (todayRadioButton.IsChecked)
            ApplyFilter(e => e.SaleDate  == today);
        else if (thisWeekRadioButton.IsChecked)
            ApplyFilter(e => e.SaleDate >= startOfWeek);
        else if (thisMonthRadioButton.IsChecked)
            ApplyFilter(e => e.SaleDate >= startOfMonth);
        else if (thisYearRadioButton.IsChecked)
            ApplyFilter(e => e.SaleDate.Year == currentYear);
    }
    private void ApplyFilter(Func<SaleVM, bool> filterCriteria)
    {
        var filtered = _allSales.Where(filterCriteria).ToList();
        _filteredSales.Clear();
        foreach (var e in filtered)
            _filteredSales.Add(e);
        SaleListView.ItemsSource = _filteredSales;

        SaleCountLabel.Text = _filteredSales.Count.ToString("N0");
        decimal total = _filteredSales.Sum(e => e.Total);
        SaleTotalLabel.Text = total.ToString("N2");
    }
    private void ApplyCustomDateFilter()
    {
        if (!customDateCheckBox.IsChecked) return;

        DateTime startDate = saleStartPicker.Date;
        DateTime endDate = saleEndPicker.Date;
        ApplyFilter(e => e.SaleDate.Date >= startDate && e.SaleDate.Date <= endDate);
    }
    private void UpdateSaleCount()
    {
        SaleCountLabel.Text = _allSales.Count.ToString("N0");
        decimal total = _allSales.Sum(e => e.Total);
        SaleTotalLabel.Text = total.ToString("N2");
    }
    private void OnStartDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
    private void OnEndDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
}