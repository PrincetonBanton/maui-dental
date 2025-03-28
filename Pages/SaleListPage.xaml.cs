using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SaleListPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private List<SaleVM> _allSales = new();

    public SaleListPage()
	{
        InitializeComponent();
        LoadSaleList();
    }
    private async void LoadSaleList()
    {
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
        bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
        try
        {
            _allSales = isApiAvailable
                ? await _apiService.GetSalesAsync() ?? new List<SaleVM>()
                : SampleData.GetSampleSales();             //Replace w offline data sync

        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
        }
        SaleListView.ItemsSource = _allSales;

    }
    private async void OnCreateSaleButtonClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new SalesPage());
    }
    private async void OnEditButtonClicked(object sender, EventArgs e)
    {

    }
    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleVM selectedSale)
        {
            await DisplayAlert("Info", $"User ID: {selectedSale.SaleId}", "OK");
            bool confirmDelete = await DisplayAlert("Confirm", "Delete this sale?", "Yes", "No");
            if (!confirmDelete) return;

            var success = await _apiService.DeleteSaleAsync(selectedSale.SaleId);
            LoadSaleList();
            await DisplayAlert(success ? "Success" : "Error", success ? "Sale deleted." : "Failed to delete sale.", "OK");
        }
    }

    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {

    }
    private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
}