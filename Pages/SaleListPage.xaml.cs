using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SaleListPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private ObservableCollection<SaleVM> _allSales = new();
    public ObservableCollection<SaleVM> Sales { get; set; } = new();

    public SaleListPage()
	{
        InitializeComponent();
        BindingContext = this; // Set the BindingContext to the page itself
        LoadSaleList();
    }
    private async void LoadSaleList()
    {
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
        bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
        try
        {
            var salesList = isApiAvailable
                ? await _apiService.GetSalesAsync() ?? new List<SaleVM>()
                : SampleData.GetSampleSales(); // Replace with offline data sync

            _allSales.Clear();
            salesList.ForEach(_allSales.Add);

            //var jsonUser = System.Text.Json.JsonSerializer.Serialize(_allSales, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            //await DisplayAlert("User Object", jsonUser, "OK");

            SaleListView.ItemsSource = _allSales;
        }
        catch (Exception)
        {
            await DisplayAlert("Error", "Failed to load sales. Please try again.", "OK");
        }
    }
    private async void OnCreateSaleButtonClicked(object sender, EventArgs e)
    {
        var salesPage = new SalesPage(null, OnSaleCreated);
        await Navigation.PushAsync(salesPage);
    }
    private void OnSaleCreated(SaleVM newSale)
    {
        _allSales.Insert(0, newSale); // Add to master list
        Sales.Insert(0, newSale);     // Add to bound list (UI)
    }

    private async void OnPayButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            string totalAmountString = selectedSale.Total.ToString("0.00");

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
            
            var jsonUser = System.Text.Json.JsonSerializer.Serialize(payment, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            await DisplayAlert("User Object", jsonUser, "OK");

            bool success = await _apiService.AddPaymentAsync(payment);

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
            await Navigation.PushAsync(new SalesPage(selectedSale));
        }
    }

    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            bool confirmDelete = await DisplayAlert("Confirm", "Delete this sale?", "Yes", "No");
            if (!confirmDelete) return;

            var success = await _apiService.DeleteSaleAsync(selectedSale.SaleId);
            LoadSaleList();
            await DisplayAlert(success ? "Success" : "Error", success ? "Sale deleted." : "Failed to delete sale.", "OK");
        }
    }
    //private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    //{
    //    var searchText = e.NewTextValue.ToLower();

    //    SaleListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
    //        ? _allSales
    //        : _allSales.Where(p => p.PatientName.ToLower().Contains(searchText)).ToList();
    //}
    //private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();

    private void OnStartDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
    private void OnEndDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();

    private void OnQuickFilterCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = quickFilterCheckBox.IsChecked;
        todayRadioButton.IsEnabled = isChecked;
        thisWeekRadioButton.IsEnabled = isChecked;
        thisMonthRadioButton.IsEnabled = isChecked;
        thisYearRadioButton.IsEnabled = isChecked;
        // Disable Custom Date Group if Quick Filter is checked
        customDateCheckBox.IsChecked = !isChecked;
        expenseStartPicker.IsEnabled = !isChecked;
        expenseEndPicker.IsEnabled = !isChecked;
    }

    private void OnCustomDateCheckedChanged(object sender, CheckedChangedEventArgs e)
    {
        bool isChecked = customDateCheckBox.IsChecked;
        expenseStartPicker.IsEnabled = isChecked;
        expenseEndPicker.IsEnabled = isChecked;
        // Disable Quick Filter Group if Custom Date is checked
        quickFilterCheckBox.IsChecked = !isChecked;
        todayRadioButton.IsEnabled = !isChecked;
        thisWeekRadioButton.IsEnabled = !isChecked;
        thisMonthRadioButton.IsEnabled = !isChecked;
        thisYearRadioButton.IsEnabled = !isChecked;

        //if (isChecked) ApplyCustomDateFilter();

    }
}