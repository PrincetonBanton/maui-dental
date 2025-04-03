using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;
using System.Collections.ObjectModel;
using DentalApp.Services.Validations;

namespace DentalApp.Pages;

public partial class SalesPage : ContentPage
{
    private SaleVM _sale;
    private readonly ApiService _apiService = new();
    private List<ProductVM> _allProducts = new();
    private readonly Action<SaleVM> _onSaleCreated;
    //public ObservableCollection<SaleItemVM> SelectedProducts { get; set; } = new();
    public ObservableCollection<SaleLine> SelectedProducts { get; set; } = new();
    public ObservableCollection<ProductVM> FilteredProducts { get; set; } = new();

    public SalesPage(SaleVM selectedSale, Action<SaleVM> onSaleCreated = null)
    {
        InitializeComponent();

        _onSaleCreated = onSaleCreated;

        if (selectedSale != null)
        {
            LoadSelectedSale(selectedSale);

        } else
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
                SaleVM newSale = new SaleVM
                {
                    SaleNo = response.SaleNo,
                    SaleDate = response.SaleDate,
                    PatientId = response.PatientId,
                    PatientName = response.PatientName,
                    DentistId = response.DentistId,
                    DentistName = response.DentistName,
                    Note = response.Note,
                    SubTotal = response.SubTotal,
                    Total = response.Total,
                    AmountDue = response.AmountDue,
                    // Map the Items collection
                    Items = response.Items?.Select(item => new SaleLine
                    {
                        Id = item.Id,
                        SaleId = item.SaleId,
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        SubTotal = item.SubTotal,
                        Total = item.Total,
                        ProductName = item.ProductName
                    }).ToList(),
                    // Map the Payments collection
                    Payments = response.Payments?.Select(payment => new Payment
                    {
                        PaymentAmount = payment.PaymentAmount,
                        PaymentType = payment.PaymentType,
                        PaymentDate = payment.PaymentDate,
                        CreatedOn = payment.CreatedOn,
                        SaleId = payment.SaleId,
                        Id = payment.Id
                    }).ToList()
                };

                // Now you can use 'newSale' as needed, for example, set it to a class property or use it directly
                _sale = newSale;  // If you have a class-level variable '_sale' to store the data

                // Optionally, display the newSale data in an alert for confirmation
                string saleDetails = $"Sale ID: {newSale.Id}\n" +
                                     $"Sale No: {newSale.SaleNo}\n" +
                                     $"Sale Date: {newSale.SaleDate}\n" +
                                     $"Patient Name: {newSale.PatientName}\n" +
                                     $"Dentist Name: {newSale.DentistName}\n" +
                                     $"Note: {newSale.Note}\n" +
                                     $"SubTotal: {newSale.SubTotal}\n" +
                                     $"Total: {newSale.Total}\n" +
                                     $"Amount Due: {newSale.AmountDue}";

                await DisplayAlert("Sale Loaded", saleDetails, "OK");
            }
            else
            {
                await DisplayAlert("Error", "Sale details not found.", "OK");
            }
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during the API call
            await DisplayAlert("Error", $"Failed to load sale details: {ex.Message}", "OK");
        }
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

        _sale ??= new SaleVM();

        _sale.SaleNo = $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}";
        _sale.SaleDate = DateTime.UtcNow;
        _sale.PatientId = ((PatientVM)PatientPicker.SelectedItem).Id;
        _sale.DentistId = ((DentistVM)DentistPicker.SelectedItem).Id;
        _sale.Note = "Patient purchased treatments";
        _sale.SubTotal = SelectedProducts.Sum(p => p.SubTotal);
        _sale.Total = _sale.SubTotal * 1.12m;                           // Assuming 12% tax
        _sale.Items = SelectedProducts.Select(p => new SaleLine
        {
            ProductId = p.ProductId,
            Quantity = p.Quantity,
            SubTotal = p.SubTotal
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
                Total = SelectedProducts.Sum(p => p.SubTotal),    
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