using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using System.Collections.ObjectModel;
using DentalApp.Services.Validations;

namespace DentalApp.Pages;

public partial class SalesPage : ContentPage
{
    private SaleVM2 _sale;
    private readonly ApiService _apiService = new();
    private List<ProductVM> _allProducts = new();
    private readonly Action<SaleVM> _onSaleCreated;
    public ObservableCollection<SaleItemVM> SelectedProducts { get; set; } = new();
    public ObservableCollection<ProductVM> FilteredProducts { get; set; } = new();

    public SalesPage(Action<SaleVM> onSaleCreated = null)
    {
        InitializeComponent();
        _onSaleCreated = onSaleCreated;
        LoadPatients();
        LoadDentists();
        LoadProducts();
        BindingContext = this;
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

    private void OnProductSelected(object sender, ItemTappedEventArgs e)
    {
        if (e.Item is ProductVM selectedProduct)
        {
            SearchBar.Text = "";
            NameEntry.Text = selectedProduct.Name;
            AmountEntry.Text = selectedProduct.Amount.ToString("N2");
            ProductListView.IsVisible = false;
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

        SearchBar.Text = "";
        NameEntry.Text = "";
        AmountEntry.Text = "";
        inputFrame.IsVisible = false;
    }

    private async void OnSaveSaleClicked(object sender, EventArgs e)
    {
        var patient = PatientPicker.SelectedItem as PatientVM;
        var dentist = DentistPicker.SelectedItem as DentistVM;
        var selectedProducts = SelectedProducts.ToList(); // Convert ObservableCollection to List

        var (isValid, errorMessage) = SalesValidationService.ValidateSale(patient?.Id, dentist?.Id, selectedProducts);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        _sale ??= new SaleVM2();

        _sale.SaleNo = $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        _sale.SaleDate = DateTime.UtcNow;
        _sale.PatientId = ((PatientVM)PatientPicker.SelectedItem).Id;
        _sale.DentistId = ((DentistVM)DentistPicker.SelectedItem).Id;
        _sale.Note = "Patient purchased treatments";
        _sale.SubTotal = SelectedProducts.Sum(p => p.Amount);
        _sale.Total = _sale.SubTotal * 1.12m;                           // Assuming 12% tax
        _sale.Items = SelectedProducts.Select(p => new SaleItemVM
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            Amount = p.Amount
        }).ToList();
        //_sale.Payment = new PaymentVM
        //{
        //    PaymentAmount = _sale.Total,
        //    PaymentType = 1, 
        //    AmountTendered = _sale.Total, 
        //    EnteredBy = 5, 
        //    PaymentDate = DateTime.UtcNow
        //};

        bool success = await _apiService.CreateSaleAsync(_sale);
        string message = success ? "Sale created successfully!" : "Failed to create sale. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            var newSale = new SaleVM
            {
                SaleId = _sale.Id,
                SaleDate = _sale.SaleDate,
                PatientName = patient.FullName,
                DentistName = dentist.FullName,
                Total = SelectedProducts.Sum(p => p.Amount),    
                Status = "Unpaid"                               
            };
            _onSaleCreated?.Invoke(newSale); // Trigger the callback with the new sale
            await Navigation.PopAsync();
        }
    }
    private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
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
    private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    private async void OnShowServiceFrame(object sender, EventArgs e) => await FrameAnimationService.ToggleVisibility(inputFrame);
}