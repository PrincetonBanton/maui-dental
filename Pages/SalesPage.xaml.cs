using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SalesPage : ContentPage
{
    private SaleVM _sale;
    private readonly ApiService _apiService = new();
    private List<ProductVM> _allProducts = new();
    public ObservableCollection<SaleItemVM> SelectedProducts { get; set; } = new();

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

        var selectedProduct = _allProducts.FirstOrDefault(p => p.Name == NameEntry.Text);
        if (selectedProduct == null) return;

        SelectedProducts.Add(new SaleItemVM
        {
            ProductId = selectedProduct.Id,
            Name = selectedProduct.Name,
            Quantity = 1, // Default quantity
            Amount = decimal.Parse(AmountEntry.Text)
        });

        ProductSearchBar.Text = "";
        NameEntry.Text = "";
        AmountEntry.Text = "";
        inputFrame.IsVisible = false;
    }

    private async void OnSaveSaleClicked(object sender, EventArgs e)
    {
        if (PatientPicker.SelectedItem == null || DentistPicker.SelectedItem == null || !SelectedProducts.Any())
        {
            await DisplayAlert("Validation Error", "Please fill in all required fields.", "OK");
            return;
        }

        _sale ??= new SaleVM();

        _sale.SaleNo = $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        _sale.SaleDate = DateTime.UtcNow;
        _sale.PatientId = ((PatientVM)PatientPicker.SelectedItem).Id;
        _sale.DentistId = ((DentistVM)DentistPicker.SelectedItem).Id;
        _sale.Note = "Patient purchased treatments";
        //_sale.SubTotal = SelectedProducts.Sum(p => p.Amount);  
        //_sale.Total = _sale.SubTotal * 1.06m; // Assuming 6% tax
        _sale.Items = SelectedProducts.Select(p => new SaleItemVM
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            Amount = p.Amount
        }).ToList();
        _sale.Payment = new PaymentVM
        {
            PaymentAmount = _sale.Total,
            PaymentType = 1, // Example: 1 for Cash
            AmountTendered = _sale.Total, 
            EnteredBy = 5, // Example user ID
            PaymentDate = DateTime.UtcNow
        };

        bool success = await _apiService.CreateSaleAsync(_sale);
        string message = success ? "Sale created successfully!" : "Failed to create sale. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success) await Navigation.PopAsync();
    }


}
