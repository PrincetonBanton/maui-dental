using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;

public partial class SupplierDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private Supplier _supplier;
    private ObservableCollection<Supplier> _allSuppliers  = new();

    public SupplierDetailsPage(ObservableCollection<Supplier> allSuppliers ,Supplier supplier = null)
    {
        InitializeComponent();
        _allSuppliers = allSuppliers;
        _supplier = supplier;
        if (supplier != null ) BindSupplierDetails();
    }

    private void BindSupplierDetails()
    {
        NameEntry.Text = _supplier.Name;
        DescriptionEditor.Text = _supplier.Description;
        FirstNameEntry.Text = _supplier.FirstName;
        LastNameEntry.Text = _supplier.LastName;
        MobileEntry.Text = _supplier.Mobile;
        PhoneEntry.Text = _supplier.Phone;
        AddressEditor.Text = _supplier.Address;
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _supplier ??= new Supplier();
        _supplier.Name = NameEntry.Text;
        _supplier.Description = DescriptionEditor.Text;
        _supplier.FirstName = FirstNameEntry.Text;
        _supplier.LastName = LastNameEntry.Text;
        _supplier.Mobile = MobileEntry.Text;
        _supplier.Phone = PhoneEntry.Text;
        _supplier.Address = AddressEditor.Text;

        var (isValid, errorMessage) = SupplierValidationService.ValidateSupplier(_supplier);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMessage, "OK");
            return;
        }

        bool success = _supplier.Id != 0
            ? await _apiService.UpdateSupplierAsync(_supplier)
            : await _apiService.CreateSupplierAsync(_supplier);
        if (success)
        {
            var updatedList = await _apiService.GetSuppliersAsync() ?? new List<Supplier>();
            _allSuppliers.Clear();
            updatedList.ForEach(_allSuppliers.Add);
        }

        string message = success
            ? (_supplier.Id != 0 ? "Supplier updated successfully!" : "Supplier created successfully!")
            : "Failed to save supplier. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", message, "OK");

        if (success) await Navigation.PopAsync();
    }
}
