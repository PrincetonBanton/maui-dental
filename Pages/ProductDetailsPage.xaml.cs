using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Models.Enum;

namespace DentalApp.Pages;

public partial class ProductDetailsPage : ContentPage
{
    private ProductVM _product;
    private readonly ApiService _apiService = new();

    public ProductDetailsPage(ProductVM product = null)
    {
        InitializeComponent();
        _product = product;
        if (_product != null)
        {
            BindProductDetails();
        }
    }

    private void BindProductDetails()
    {
        NameEntry.Text = _product.Name;
        ProductCodeEntry.Text = _product.ProductCode;
        DescriptionEditor.Text = _product.Description;
        ProductTypePicker.SelectedItem = _product.ProductType;
        AmountEntry.Text = _product.Amount.ToString();
        MinPriceEntry.Text = _product.MinPrice.ToString();
        MaxPriceEntry.Text = _product.MaxPrice.ToString();
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _product ??= new ProductVM();

        _product.Name = NameEntry.Text;
        _product.ProductCode = ProductCodeEntry.Text;
        _product.Description = DescriptionEditor.Text;
        _product.Amount = double.TryParse(AmountEntry.Text, out var amount) ? amount : 0;
        _product.MinPrice = double.TryParse(MinPriceEntry.Text, out var minPrice) ? minPrice : null;
        _product.MaxPrice = double.TryParse(MaxPriceEntry.Text, out var maxPrice) ? maxPrice : null;


        //var (isValid, errorMessage) = ProductValidationService.ValidateProduct(_product);

        //if (!isValid)
        //{
        //    await DisplayAlert("Validation Error", errorMessage, "OK");
        //    return;
        //}

        bool success = _product.Id != 0
            ? await _apiService.UpdateProductAsync(_product)
            : await _apiService.CreateProductAsync(_product);

        string message = success
            ? (_product.Id != 0 ? "Product updated successfully!" : "Product created successfully!")
            : "Failed to save product. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success)
            await Navigation.PopAsync();
    }
}
