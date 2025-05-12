using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class ExpensePage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private Expense _expense;
        private List<Expense> _allExpenses = new();
        private ObservableCollection<Expense> _expenses = new();

        public ExpensePage(Expense expense = null)
        {
            InitializeComponent();
            ExpenseListView.ItemsSource = _expenses;
            _expense = expense ?? new Expense();
            //BindExpenseDetails();
            LoadExpenseCategories();
            LoadExpenseList();
        }
        private async void LoadExpenseCategories() =>ExpenseCategoryPicker.ItemsSource = await _apiService.GetExpenseCategoryAsync();

        private void BindExpenseDetails()
        {
            DescriptionEntry.Text = _expense.Description;
            AmountEntry.Text = _expense.Amount.ToString("N2");
            ExpenseDatePicker.Date = _expense.ExpenseDate;
            ExpenseCategoryPicker.SelectedItem = (ExpenseCategoryPicker.ItemsSource as List<ExpenseCategory>)?
                .FirstOrDefault(c => c.Id == _expense.ExpenseCategoryId);
        }

        private async void LoadExpenseList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                _allExpenses = isApiAvailable
                    ? await _apiService.GetExpensesAsync() ?? new List<Expense>()
                    : SampleData.GetSampleExpenses(); 

                _expenses.Clear();
                foreach (var expense in _allExpenses)
                    _expenses.Add(expense);

                UpdateExpenseCount();
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load expenses. Please try again.", "OK");
            }
            ExpenseListView.ItemsSource = _allExpenses; 
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            _expense ??= new Expense();
            _expense.Description = DescriptionEntry.Text;
            _expense.Amount = ParseDecimal(AmountEntry.Text);
            _expense.ExpenseDate = ExpenseDatePicker.Date;
            _expense.EnteredBy = 1;  //Temporary
            _expense.ExpenseCategoryId = ExpenseCategoryPicker.SelectedItem is ExpenseCategory selectedCategory ? selectedCategory.Id : 0;

            decimal ParseDecimal(string text) => decimal.TryParse(text, out var value) ? value : 0.00m;

            var (isValid, errorMessage) = ExpenseValidationService.ValidateExpense(_expense);
            if (!isValid)
            {
                await DisplayAlert("Validation Error", errorMessage, "OK");
                return;
            }

            var success = _expense.Id == 0
                          ? await _apiService.CreateExpenseAsync(_expense)
                          : await _apiService.UpdateExpenseAsync(_expense);

            LoadExpenseList();

            string message = success
                ? (_expense.Id != 0 ? "Product updated successfully!" : "Product created successfully!")
                : "Failed to save product. Please try again.";

            await DisplayAlert(success ? "Success" : "Error", message, "OK");
            inputFrame.IsVisible = false;
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Expense selectedExpense)
            {
                _expense = selectedExpense; // Set the selected expense

                DescriptionEntry.Text = _expense.Description;
                AmountEntry.Text = _expense.Amount.ToString("N2");
                ExpenseDatePicker.Date = _expense.ExpenseDate;
                ExpenseCategoryPicker.SelectedItem = (ExpenseCategoryPicker.ItemsSource as List<ExpenseCategory>)?
                    .FirstOrDefault(c => c.Id == _expense.ExpenseCategoryId);

                await FrameAnimationService.ToggleVisibility(inputFrame);
            }
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Expense selectedExpense)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeleteExpenseAsync(selectedExpense.Id);
                LoadExpenseList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
            }
        }
        private void OnStartDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
        private void OnEndDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();

        private void OnQuickFilterCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            bool isChecked = quickFilterCheckBox.IsChecked;
            todayRadioButton.IsEnabled = isChecked;
            thisWeekRadioButton.IsEnabled = isChecked;
            thisMonthRadioButton.IsEnabled = isChecked;
            thisYearRadioButton.IsEnabled = isChecked;
            // Disable Custom Date Group if Quick Filter is checked
            customDateCheckBox.IsChecked = !isChecked;
            expenseStartPicker.IsEnabled = !isChecked;
            expenseEndPicker.IsEnabled = !isChecked;
        }

        private void OnCustomDateCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            bool isChecked = customDateCheckBox.IsChecked;
            expenseStartPicker.IsEnabled = isChecked;
            expenseEndPicker.IsEnabled = isChecked;
            // Disable Quick Filter Group if Custom Date is checked
            quickFilterCheckBox.IsChecked = !isChecked;
            todayRadioButton.IsEnabled = !isChecked;
            thisWeekRadioButton.IsEnabled = !isChecked;
            thisMonthRadioButton.IsEnabled = !isChecked;
            thisYearRadioButton.IsEnabled = !isChecked;

            if (isChecked) ApplyCustomDateFilter();
  
        }
        private void ApplyFilter(Func<Expense, bool> filterCriteria)
        {
            var filteredExpenses = _allExpenses
                .Where(filterCriteria)
                .OrderByDescending(expense => expense.ExpenseDate)
                .ToList();

            UpdateExpenseList(filteredExpenses);
        }

        private void UpdateExpenseList(List<Expense> filteredExpenses)
        {
            _expenses.Clear(); // Clears the current list
            foreach (var expense in filteredExpenses)
                _expenses.Add(expense); // Automatically updates the UI

            ExpenseListView.ItemsSource = _expenses;
            UpdateExpenseCount();
        }
        private void UpdateExpenseCount() => ExpenseCountLabel.Text = $"{_expenses.Count}";

        private void OnNumericEntryChanged(object sender, TextChangedEventArgs e) => NumericValidationService.OnNumericEntryChanged(sender, e);
        private async void OnShowExpenseFrame(object sender, EventArgs e) => await FrameAnimationService.ToggleVisibility(inputFrame);
        private async void OnShowCategoryFrame(object sender, EventArgs e) => await FrameAnimationService.ToggleVisibility(CategoryFrame);
    }
}
