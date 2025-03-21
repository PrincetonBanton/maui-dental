using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SalesPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private List<ProductVM> _allProducts = new();
    public ObservableCollection<object> SelectedProducts { get; set; } = new();
    public ObservableCollection<ProductVM> FilteredProducts { get; set; } = new();

    public SalesPage()
    {
        InitializeComponent();
        LoadPatients();
        LoadDentists();
        LoadProducts();
        BindingContext = this; // Bind to UI
    }

    private async void LoadPatients()
    {
        try { PatientPicker.ItemsSource = await _apiService.GetPatientsAsync(); }
        catch { await DisplayAlert("Error", "Failed to load patients.", "OK"); }
    }

    private async void LoadDentists()
    {
        try { DentistPicker.ItemsSource = await _apiService.GetDentistsAsync(); }
        catch { await DisplayAlert("Error", "Failed to load dentists.", "OK"); }
    }

    private async void LoadProducts()
    {
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
        bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
        try
        {
            _allProducts = isApiAvailable
                ? await _apiService.GetProductsAsync() ?? new List<ProductVM>()
                : SampleData.GetSampleProducts();               //Replace w offline data sync
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
        }
    }

    private void OnProductSearchTextChanged(object sender, TextChangedEventArgs e)
    {
        string searchText = e.NewTextValue?.ToLower() ?? "";
        FilteredProducts.Clear();

        if (string.IsNullOrEmpty(searchText))
        {
            ProductListView.IsVisible = false;
            return;
        }

        var filtered = _allProducts.Where(p => p.Name.ToLower().Contains(searchText)).ToList();
        foreach (var product in filtered) FilteredProducts.Add(product);

        ProductListView.IsVisible = filtered.Any();
    }

    private void OnProductSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ProductVM selectedProduct)
        {
            ProductSearchBar.Text = "";
            NameEntry.Text = selectedProduct.Name;
            AmountEntry.Text = selectedProduct.Amount.ToString("N2");
            ProductListView.IsVisible = false;
        }
    }
    private async void OnShowServiceFrame(object sender, EventArgs e)
    {
        if (!inputFrame.IsVisible)
        {
            inputFrame.TranslationY = -500; // Start above the screen
            inputFrame.IsVisible = true;
            await inputFrame.TranslateTo(0, 0, 250, Easing.SinInOut); // Animate down
        }
        else
        {
            await inputFrame.TranslateTo(0, -500, 250, Easing.SinInOut); // Animate back up
            inputFrame.IsVisible = false;
        }
    }
    private void OnAddServiceClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(NameEntry.Text) || string.IsNullOrWhiteSpace(AmountEntry.Text))
        {
            DisplayAlert("Error", "Please enter product/service and amount.", "OK");
            return;
        }

        // Find the selected product from the original list
        var selectedProduct = _allProducts.FirstOrDefault(p => p.Name == NameEntry.Text);
        if (selectedProduct == null) return;

        // Add the selected product as an object with required properties
        SelectedProducts.Add(new
        {
            ProductId = selectedProduct.Id,
            Name = selectedProduct.Name,
            Amount = decimal.Parse(AmountEntry.Text),
            Quantity = 1 // Default quantity
        });

        // Clear input fields
        ProductSearchBar.Text = "";
        NameEntry.Text = "";
        AmountEntry.Text = "";
        inputFrame.IsVisible = false;
    }
}
