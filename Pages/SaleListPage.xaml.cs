using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SaleListPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private ObservableCollection<SaleVM> _allSales = new();
    private ObservableCollection<ProductVM> _allProducts = new();
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
                   ? await _apiService.GetSalesAsync() ?? new List<SaleVM>()
                   : SampleData.GetSampleSales();
            _allSales.Clear();
            saleList.ForEach(_allSales.Add);
            SaleListView.ItemsSource = _allSales;

            //var jsonUser = System.Text.Json.JsonSerializer.Serialize(_allSales, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
            //await DisplayAlert("User Object", jsonUser, "OK");
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
            await Navigation.PushAsync(new SaleDetailsPage(_allSales, selectedSale));
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
}