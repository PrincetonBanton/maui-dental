using DentalApp.Models;
using DentalApp.Models.Enum;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;


namespace DentalApp.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private ProductVM _product;
    private ObservableCollection<ProductVM> _allProducts = new();
    private bool _isEditMode;

    public ProductDetailsPage(ObservableCollection<ProductVM> allProducts, ProductVM product = null)
    {
        InitializeComponent();
        _allProducts = allProducts;
        _product = product;
        ProductTypePicker.ItemsSource = Enum.GetValues<ProductType>().ToList();

        if (_product != null)
        {
            _isEditMode = true;
            BindProductDetails();
        }
    }

    private void BindProductDetails()
    {
        ProductTypePicker.ItemsSource = Enum.GetValues<ProductType>().ToList();
        NameEntry.Text = _product.Name;
        ProductCodeEntry.Text = _product.ProductCode;
        DescriptionEditor.Text = _product.Description;
        ProductTypePicker.SelectedItem = _product.ProductType;
        AmountEntry.Text = _product.Amount.ToString("N2");
        //MinPriceEntry.Text = _product.MinPrice.ToString("N2");
        //MaxPriceEntry.Text = _product.MaxPrice.ToString("N2");
        MinPriceEntry.Text = _product.Amount.ToString("N2");
        MaxPriceEntry.Text = _product.Amount.ToString("N2");
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _product ??= new ProductVM();

        _product.Name = NameEntry.Text;
        _product.ProductCode = ProductCodeEntry.Text;
        _product.Description = DescriptionEditor.Text;
        _product.ProductType = (ProductType)ProductTypePicker.SelectedItem;
        _product.Amount = ParseDecimal(AmountEntry.Text);
        _product.MinPrice = ParseDecimal(MinPriceEntry.Text);
        _product.MaxPrice = ParseDecimal(MaxPriceEntry.Text);

        decimal ParseDecimal(string text) => decimal.TryParse(text, out var value) ? value : 0.00m;

        var jsonUser = System.Text.Json.JsonSerializer.Serialize(_product, new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        await DisplayAlert("User Object", jsonUser, "OK");

        var (isValid, errorMessage) = ProductValidationService.ValidateProduct(_product);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        bool success = _product.Id != 0
            ? await _apiService.UpdateProductAsync(_product)
            : await _apiService.CreateProductAsync(_product);

            if (success)
            {
                var updatedList = await _apiService.GetProductsAsync() ?? new List<ProductVM>();
                _allProducts.Clear();
                updatedList.ForEach(_allProducts.Add);
            }

        string message = success
            ? (_product.Id != 0 ? "Product updated successfully!" : "Product created successfully!")
            : "Failed to save product. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
        {
            _product = null;
            await Navigation.PopAsync();
        }
    }

    private void OnNumericEntryChanged(object sender, TextChangedEventArgs e)
    {
        NumericValidationService.OnNumericEntryChanged(sender, e);
    }
}
