using CommunityToolkit.Maui.Converters;
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
    private ExpenseCategory _category;
    private ObservableCollection<ExpenseCategory> _allCategories = new();

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

    private async Task LoadExpenseCategories()
    {
        var categories = await _apiService.GetExpenseCategoryAsync();
        _allCategories = new ObservableCollection<ExpenseCategory>(categories);

        ExpenseCategoryPicker.ItemsSource = _allCategories;
        CategoryListView.ItemsSource = _allCategories;

        if (_expense != null)
        {
            var selectedCategory = _allCategories.FirstOrDefault(c => c.Id == _expense.ExpenseCategoryId);
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
            var updatedList = await _apiService.GetExpensesAsync() ?? new List<Expense>();
            _allExpenses.Clear();
            updatedList.ForEach(_allExpenses.Add);
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

    //-------- CATEGORY CODES --------------
    private async void SaveCategory_Clicked(object sender, EventArgs e)
    {
        _category ??= new ExpenseCategory();
        _category.Name = CategoryEntry.Text;
        _category.Description = DescriptionEditor.Text;

        var (isValid, errorMsg) = ExpenseValidationService.ValidateCategory(_category);
        if (!isValid)
        {
            await DisplayAlert("Validation Error", errorMsg, "OK");
            return;
        }

        bool success = _category.Id == 0
            ? await _apiService.CreateExpenseCategoryAsync(_category)
            : await _apiService.UpdateExpenseCategoryAsync(_category);

        string msg = success
            ? (_category.Id != 0 ? "Category updated successfully!" : "Category created successfully!")
            : "Failed to save category. Please try again.";

        await DisplayAlert(success ? "Success" : "Error", msg, "OK");

        if (success)
        {
            CategoryEntry.Text = string.Empty;
            DescriptionEditor.Text = string.Empty;
            _category = null;
            await LoadExpenseCategories();
            await FrameAnimationService.ToggleVisibility(CategoryFrame);
        }
    }
    private async void OnEditButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ExpenseCategory selectedCategory)
        {
            _category = selectedCategory;
            CategoryEntry.Text = _category.Name;
            DescriptionEditor.Text = _category.Description;

        }
    }
    private async void OnDeleteButtonClicked(object sender, EventArgs e)
    {
        if (sender is ImageButton button && button.BindingContext is ExpenseCategory selectedCategory)
        {
            bool confirmDelete = await DisplayAlert("Confirm", "Delete this category?", "Yes", "No");
            if (!confirmDelete) return;

            var success = await _apiService.DeleteExpenseCategoryAsync(selectedCategory.Id);
            if (success)
            {
                _allCategories.Remove(selectedCategory);
                CategoryListView.ItemsSource = null; // Optional: Reset first
                CategoryListView.ItemsSource = _allCategories;
                await LoadExpenseCategories();
                await FrameAnimationService.ToggleVisibility(CategoryFrame);
            }

            await DisplayAlert(success ? "Success" : "Error", success ? "Category deleted." : "Failed to delete category.", "OK");
        }
    }

}
