using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;
using System.Text.Json;

namespace DentalApp.Pages;

public partial class SaleDetailsPage : ContentPage
{
    private readonly DentistService _dentistService = new();
    private readonly PatientService _patientService = new();
    private readonly ProductService _productService = new();
    private readonly PaymentService _paymentService = new();
    private readonly SaleService _saleService = new();
    private SaleVM _sale;
    private ObservableCollection<SaleVM> _allSales = new();
    private ObservableCollection<ProductVM> _allProducts = new();
    private ObservableCollection<SaleLine> SelectedProducts = new();

    public SaleDetailsPage(ObservableCollection<SaleVM> allSales, SaleVM sale = null)
    {
        InitializeComponent();

        string itemsJson = JsonSerializer.Serialize(sale, new JsonSerializerOptions { WriteIndented = true });
        DisplayAlert("Sale Items (API Format)", itemsJson, "OK");

        _allSales = allSales;
        _sale = sale;
        AddServicesButton.IsVisible = _sale == null;
        SaveButtonGroup.IsVisible = _sale == null;
        if (_sale != null)
        {
            LoadSelectedSale(sale);
            if (_sale.Status == 0)
            {
                LoadProducts();
                AddServicesButton.IsVisible = true;
                SaveButtonGroup.IsVisible = true;
            }
        }
        else
        {
            LoadPatients();
            LoadDentists();
            LoadProducts();
        }
    }
    private async void LoadSelectedSale(SaleVM selectedSale)
    {
        try
        {
            var response = await _saleService.GetSaleDetailAsync(selectedSale.SaleId);

            if (response != null)
            {
                string itemsJson = JsonSerializer.Serialize(response, new JsonSerializerOptions { WriteIndented = true });
                await DisplayAlert("Sale Items (API Format)", itemsJson, "OK");

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
        try { PatientPicker.ItemsSource = await _patientService.GetPatientsAsync(); }
        catch { await DisplayAlert("Error", "Failed to load patients.", "OK"); }
    }
    private async Task LoadDentists()
    {
        try { DentistPicker.ItemsSource = await _dentistService.GetDentistsAsync(); }
        catch { await DisplayAlert("Error", "Failed to load dentists.", "OK"); }
    }

    private async void LoadProducts()
    {
        await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
        bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
        try
        {
            var productList = isApiAvailable
                ? await _productService.GetProductsAsync() ?? new List<ProductVM>()
                : SampleData.GetSampleProducts();

            _allProducts.Clear();
            productList.ForEach(_allProducts.Add);
            ProductListView.ItemsSource = _allProducts;
        }
        catch (Exception ex)
        {
            await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
        }

    }
    private void OnAddServiceClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ProductVM selectedProduct)
        {
            
            SelectedProducts.Add(new SaleLine
            {
                SaleId = _sale?.Id ?? 0,
                ProductId = selectedProduct.Id,
                Quantity = 1,
                SubTotal = selectedProduct.Amount,
                Total = selectedProduct.Amount,
                ProductName = selectedProduct.Name  
            });
            AvailProductListView.ItemsSource = null;
            AvailProductListView.ItemsSource = SelectedProducts;
            inputFrame.IsVisible = false;
        }
    }
    private void QtyAddClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleLine saleLine)
        {
            saleLine.Quantity += 1;
            saleLine.Total = saleLine.Quantity * saleLine.SubTotal; // recalculate from unit price
            RefreshSelectedProducts();
        }
    }

    private void QtyLessClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleLine saleLine)
        {
            if (saleLine.Quantity > 1)
            {
                saleLine.Quantity -= 1;
                saleLine.Total = saleLine.Quantity * saleLine.SubTotal; // recalculate from unit price
                RefreshSelectedProducts();
            }
            else
            {
                SelectedProducts.Remove(saleLine);
            }

            RefreshSelectedProducts();
        }
    }
    private void QtyRemoveClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is SaleLine saleLine)
        {
            SelectedProducts.Remove(saleLine);
            RefreshSelectedProducts();
        }
    }


    private void RefreshSelectedProducts()
    {
        AvailProductListView.ItemsSource = null;
        AvailProductListView.ItemsSource = SelectedProducts;
    }

    private async void OnSaveSaleClicked(object sender, EventArgs e)
    {
        bool saleSaved = await SaveSaleAsync(0);
        if (saleSaved) await Navigation.PopAsync();
    }

    private async void OnSavePayClicked(object sender, EventArgs e)
    {
        var subtotal = SelectedProducts.Sum(p => p.SubTotal);
        string input = await DisplayPromptAsync("Enter Payment", "Enter payment amount:", "OK", "Cancel", initialValue: subtotal.ToString("N2"), keyboard: Keyboard.Numeric);

        if (string.IsNullOrWhiteSpace(input) || !decimal.TryParse(input, out decimal amount))
        {
            await DisplayAlert("Error", "Invalid payment amount entered.", "OK");
            return;
        }

        bool saleSaved = await SaveSaleAsync(amount);
        if (saleSaved)
        {
            try
            {
                var allSales = await _saleService.GetSalesAsync();
                var latestSale = allSales.OrderByDescending(s => s.SaleId).FirstOrDefault();
                if (latestSale != null) await SavePayment(amount, latestSale.SaleId);
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", $"Failed to fetch latest sale: {ex.Message}", "OK");
            }
            await Navigation.PopAsync();
        }
    }

    private async Task SavePayment(decimal amount, int saleId)
    {
        var payment = new Payment
        {
            SaleId = saleId,
            PaymentAmount = amount,
            AmountTendered = amount,
            PaymentType = 0,                // Assuming 0 = Cash
            EnteredBy = 41,                 // Replace with actual user ID if needed
            PaymentDate = DateTime.Now
        };

        //var jsonUser = JsonSerializer.Serialize(payment, new JsonSerializerOptions { WriteIndented = true });
        //await DisplayAlert("Payment Object", jsonUser, "OK");

        bool success = await _paymentService.AddPaymentAsync(payment);
        //string message = success ? "Payment added successfully!" : "Failed to add payment.";
        //await DisplayAlert(success ? "Success" : "Error", message, "OK");
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
        _sale ??= CreateSale.BuildSale(patient, dentist, SelectedProducts, paymentAmount, TreatmentDatePicker.Date);

        //var jsonUser = JsonSerializer.Serialize(_sale, new JsonSerializerOptions { WriteIndented = true });
        //await DisplayAlert("User Object", jsonUser, "OK");

        bool success = await _saleService.CreateSaleAsync(_sale);
        string message = success ? "Sale saved successfully!" : "Failed to save sale.";
        await DisplayAlert(success ? "Success" : "Error", message, "OK");

       return success;
    }

    private async void OnShowServiceFrame(object sender, EventArgs e) => await FrameAnimationService.ToggleVisibility(inputFrame);

}