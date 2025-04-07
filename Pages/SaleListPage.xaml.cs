using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SaleListPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private List<SaleVM> _allSales = new();
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
            var sales = isApiAvailable
                ? await _apiService.GetSalesAsync() ?? new List<SaleVM>()
                : SampleData.GetSampleSales(); // Replace with offline data sync

            _allSales = sales; // <-- This line was missing!
            Sales.Clear();
            foreach (var sale in sales)
            {
                Sales.Add(sale); // Add new items to the ObservableCollection
            }
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
        // Add the newly created sale to the list
        Sales.Insert(0, newSale);
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
    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
    {
        var searchText = e.NewTextValue.ToLower();

        SaleListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
            ? _allSales
            : _allSales.Where(p => p.PatientName.ToLower().Contains(searchText)).ToList();
    }
    private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
}