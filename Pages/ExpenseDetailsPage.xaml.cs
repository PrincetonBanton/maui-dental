using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using Microsoft.Maui.Graphics;
using System.Collections.ObjectModel;

namespace DentalApp.Pages;
public partial class ExpenseDetailsPage : ContentPage
{
    private readonly ApiService _apiService = new();
    private Expense _expense;
    private ObservableCollection<Expense> _allExpenses = new();
    private ObservableCollection<Expense> _filteredExpenses = new();


    public ExpenseDetailsPage(ObservableCollection<Expense> allExpenses, Expense expense = null)
    {
        InitializeComponent();
        _allExpenses = allExpenses;
        _expense = expense;
        if (_expense != null) BindExpenseDetails();
        LoadExpenseCategories();
    }

    private void BindExpenseDetails()
    {
        DescriptionEntry.Text = _expense.Description;
        AmountEntry.Text = _expense.Amount.ToString("0.00");
        ExpenseDatePicker.Date = _expense.ExpenseDate;
    }

    private async void LoadExpenseCategories()
    {
        var categories = await _apiService.GetExpenseCategoryAsync();
        ExpenseCategoryPicker.ItemsSource = categories;

        if (_expense != null)
        {
            var selectedCategory = categories.FirstOrDefault(c => c.Id == _expense.ExpenseCategoryId);
            if (selectedCategory != null)
                ExpenseCategoryPicker.SelectedItem = selectedCategory;
        }
    }

    private async void SaveButton_Clicked(object sender, EventArgs e)
    {
        _expense ??= new Expense();
        _expense.Description = DescriptionEntry.Text;
        _expense.Amount = decimal.TryParse(AmountEntry.Text, out var amt) ? amt : 0;
        _expense.ExpenseDate = ExpenseDatePicker.Date;
        _expense.EnteredBy = 1; // Temp
        _expense.ExpenseCategoryId = ExpenseCategoryPicker.SelectedItem is ExpenseCategory cat ? cat.Id : 0;

        var (isValid, errorMsg) = ExpenseValidationService.ValidateExpense(_expense);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMsg, "OK");
            return;
        }

        var success = _expense.Id == 0
            ? await _apiService.CreateExpenseAsync(_expense)
            : await _apiService.UpdateExpenseAsync(_expense);

        if (success)
        {
            // Update existing or add new
            var index = _allExpenses.ToList().FindIndex(e => e.Id == _expense.Id);
            if (index >= 0)
            {
                _allExpenses[index] = _expense;
            }
            else
            {
                _allExpenses.Add(_expense);
            }
        }

        string msg = success
            ? (_expense.Id != 0 ? "Expense updated successfully!" : "Expense created successfully!")
            : "Failed to save expense. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", msg, "OK");

        if (success)
        {
            _expense = null;
            await Navigation.PopAsync();
        }
    }

    private async void OnShowCategoryFrame(object sender, EventArgs e) => await FrameAnimationService.ToggleVisibility(CategoryFrame);
    private void OnNumericEntryChanged(object sender, TextChangedEventArgs e) => NumericValidationService.OnNumericEntryChanged(sender, e);
}
