using DentalApp.Models;
using DentalApp.Models.Enum;
using DentalApp.Data;
using DentalApp.Services;
using System.Collections.ObjectModel;
using DentalApp.Services.Validations;
using System.Text.Json;

namespace DentalApp.Pages;

public partial class SalesPage : ContentPage
{
    private SaleVM _sale;
    private readonly ApiService _apiService = new();
    private List<ProductVM> _allProducts = new();
    private readonly Action<SaleVM> _onSaleCreated;
    public ObservableCollection<SaleLine> SelectedProducts { get; set; } = new();
    public ObservableCollection<ProductVM> FilteredProducts { get; set; } = new();

    public bool IsNewSale { get; set; }
    public bool IsExistingSale { get; set; }

    public SalesPage(SaleVM selectedSale, Action<SaleVM> onSaleCreated = null)
    {
        InitializeComponent();

        _onSaleCreated = onSaleCreated;

        if (selectedSale != null)
        {
            LoadSelectedSale(selectedSale);
        } 
        else
        {
            LoadPatients();
            LoadDentists();
            LoadProducts();
        }
        BindingContext = this;
    }
    private async void LoadSelectedSale(SaleVM selectedSale)
    {
        try
        {
            var response = await _apiService.GetSaleDetailAsync(selectedSale.SaleId);

            if (response != null)
            {
                //string itemsJson = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                //await DisplayAlert("Sale Items (API Format)", itemsJson, "OK");

                TreatmentDatePicker.Date = response.SaleDate;
                await LoadPatients();
                await LoadDentists();
                var selectedPatient = PatientPicker.ItemsSource.OfType<PatientVM>().FirstOrDefault(p => p.Id == response.PatientId);
                var selectedDentist = DentistPicker.ItemsSource.OfType<DentistVM>().FirstOrDefault(d => d.Id == response.DentistId);
                PatientPicker.SelectedItem = selectedPatient;
                DentistPicker.SelectedItem = selectedDentist;

                AvailProductListView.ItemsSource = new ObservableCollection<SaleVM.SaleItem>(response.Items.Select(item => new SaleVM.SaleItem
                {
                    SaleId = item.SaleId,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    SubTotal = item.SubTotal,
                    Total = item.Total,
                    ProductName = item.ProductName
                }));

            }
            else
            {
                await DisplayAlert("Error", "Sale details not found.", "OK");
            }
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", $"Failed to load sale details: {ex.Message}", "OK");
        }
    }

    private async Task LoadPatients()
    {
        try { PatientPicker.ItemsSource = await _apiService.GetPatientsAsync(); }
        catch { await DisplayAlert("Error", "Failed to load patients.", "OK"); }
    }
    private async Task LoadDentists()
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
                : SampleData.GetSampleProducts();              
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

        if (!decimal.TryParse(AmountEntry.Text, out decimal amount))
        {
            DisplayAlert("Error", "Invalid amount entered. Please enter a valid number.", "OK");
            return;
        }

        var selectedProduct = _allProducts.FirstOrDefault(p => p.Name == NameEntry.Text);
        if (selectedProduct == null) return;

        SelectedProducts.Add(new SaleLine
        {
            SaleId = _sale?.Id ?? 0,
            ProductId = selectedProduct.Id,  
            Quantity = 1,
            SubTotal = amount,
            Total = amount * 1.12m,
            ProductName = selectedProduct.Name  // Set the product name    
        });

        SearchBar.Text = "";
        NameEntry.Text = "";
        AmountEntry.Text = "";
        inputFrame.IsVisible = false;
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

    private async void OnSaveSaleClicked(object sender, EventArgs e)
    {
        await SaveSaleAsync(0); // No payment yet
    }

    private async void OnSavePayClicked(object sender, EventArgs e)
    {
        var subtotal = SelectedProducts.Sum(p => p.SubTotal);
        string input = await DisplayPromptAsync("Enter Payment","Enter payment amount:","OK","Cancel", initialValue: subtotal.ToString("N2"), keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(input) || !decimal.TryParse(input, out decimal amount))
        {
            await DisplayAlert("Error", "Invalid payment amount entered.", "OK");
            return;
        }

        await SaveSaleAsync(amount);
    }


    private async Task<bool> SaveSaleAsync(decimal paymentAmount)
    {
        var patient = PatientPicker.SelectedItem as PatientVM;
        var dentist = DentistPicker.SelectedItem as DentistVM;
        var selectedProducts = SelectedProducts.ToList();

        var (isValid, errorMessage) = SalesValidationService.ValidateSale(patient?.Id, dentist?.Id, selectedProducts);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return false;
        }

        /*sale ??= BuildSale(paymentAmount);*/
        _sale ??= CreateSale.BuildSale(patient, dentist, SelectedProducts, paymentAmount);

        bool success = await _apiService.CreateSaleAsync(_sale);
        string message = success ? "Sale saved successfully!" : "Failed to save sale.";
        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            //short status = paymentAmount >= _sale.Total ? 2 : paymentAmount == 0 ? 0 : 1;

            var newSale = new SaleVM
            {
                SaleId = _sale.Id,
                SaleDate = _sale.SaleDate,
                PatientName = patient.FullName,
                DentistName = dentist.FullName,
                Total = _sale.Total,
                Status = 0
            };

            _onSaleCreated?.Invoke(newSale);
            await Navigation.PopAsync();
        }

        return success;
    }

}