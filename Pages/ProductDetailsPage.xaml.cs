using System.Text.RegularExpressions;
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
        _product = product ?? new ProductVM();
        BindProductDetails();
        ProductTypePicker.ItemsSource = Enum.GetValues<ProductType>().ToList();
    }

    private void BindProductDetails()
    {
        ProductTypePicker.ItemsSource = Enum.GetValues<ProductType>().ToList();
        NameEntry.Text = _product.Name;
        ProductCodeEntry.Text = _product.ProductCode;
        DescriptionEditor.Text = _product.Description;
        ProductTypePicker.SelectedItem = _product.ProductType;
        AmountEntry.Text = _product.Amount.ToString("N2");
        MinPriceEntry.Text = _product.MinPrice.ToString("N2");
        MaxPriceEntry.Text = _product.MaxPrice.ToString("N2");
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

        var (isValid, errorMessage) = ProductValidationService.ValidateProduct(_product);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        bool success = _product.Id != 0
            ? await _apiService.UpdateProductAsync(_product)
            : await _apiService.CreateProductAsync(_product);

        string message = success
            ? (_product.Id != 0 ? "Product updated successfully!" : "Product created successfully!")
            : "Failed to save product. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success) await Navigation.PopAsync();
    }

    private void OnNumericEntryChanged(object sender, TextChangedEventArgs e)
    {
        if (sender is Entry entry)
        {
            string newText = entry.Text;
            if (!IsValidNumericInput(newText)) entry.Text = e.OldTextValue;
        }
    }

    private bool IsValidNumericInput(string text)
    {
        if (string.IsNullOrEmpty(text)) return true;
        return Regex.IsMatch(text, @"^\d*\.?\d*$");
    }
}
