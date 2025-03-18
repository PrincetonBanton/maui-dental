using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
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
            //categoryPicker.ItemsSource = Pages.GetValues<ExpenseCategory>().ToList();
            ExpenseListView.ItemsSource = _expenses;
            _expense = expense ?? new Expense();
            BindExpenseDetails();
            LoadExpenseCategories();
            LoadExpenseList();
        }
        private async void LoadExpenseCategories()
        {
            try
            {
                List<ExpenseCategory> categories = await _apiService.GetExpenseCategoryAsync(); // Fetch from API
                ExpenseCategoryPicker.ItemsSource = categories;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load categories. Please try again.", "OK");
            }
        }

        private void BindExpenseDetails()
        {
            descriptionEntry.Text = _expense.Description;
            amountEntry.Text = _expense.Amount > 0 ? _expense.Amount.ToString() : string.Empty;
            expenseDatePicker.Date = _expense.ExpenseDate != default ? _expense.ExpenseDate : DateTime.Now;
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
        private void UpdateExpenseCount()
        {
            ExpenseCountLabel.Text = $"{_expenses.Count}";
        }
        private async void OnAddExpenseClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new ExpenseDetailsPage());
            if (!inputFrame.IsVisible)
            {
                inputFrame.TranslationY = -500; // Start above the screen
                inputFrame.IsVisible = true;
                await inputFrame.TranslateTo(0, 0, 250, Easing.SinInOut); // Animate down
            }
            else
            {
                await inputFrame.TranslateTo(0, -500, 250, Easing.SinInOut); // Animate back up
                inputFrame.IsVisible = false;
            }
        }

        private async void SaveButton_Clicked(object sender, EventArgs e)
        {
            _expense ??= new Expense();


            _expense.Description = descriptionEntry.Text;
            _expense.Amount = decimal.Parse(amountEntry.Text);
            _expense.ExpenseDate = expenseDatePicker.Date;
            _expense.EnteredBy = 1;  //Temporary
            _expense.ExpenseCategoryId = ((ExpenseCategory)ExpenseCategoryPicker.SelectedItem).Id;

            //decimal ParseDecimal(string text) => decimal.TryParse(text, out var value) ? value : 0.00m;

            //var (isValid, errorMessage) = ProductValidationService.ValidateProduct(_product);
            //if (!isValid)
            //{
            //    await DisplayAlert("Validation Error", errorMessage, "OK");
            //    return;
            //}

            await DisplayAlert("Confirm Expense",
                $"Description: {_expense.Description}\n" +
                $"Amount: {_expense.Amount:C}\n" +
                $"Date: {_expense.ExpenseDate:MM/dd/yyyy}\n" +
                $"Category ID: {_expense.ExpenseCategoryId}",
                "OK");
            var success = _expense.Id == 0
                          ? await _apiService.CreateExpenseAsync(_expense)
                          : await _apiService.UpdateExpenseAsync(_expense);

            LoadExpenseList();

            string message = success
                ? (_expense.Id != 0 ? "Product updated successfully!" : "Product created successfully!")
                : "Failed to save product. Please try again.";

            await DisplayAlert(success ? "Success" : "Error", message, "OK");
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
        }

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
        }

        private void OnQuickFilterRadioButtonChanged(object sender, CheckedChangedEventArgs e)
        {
            if (quickFilterCheckBox.IsChecked)
            {
                if (todayRadioButton.IsChecked)
                {
                    ApplyTodayFilter();
                }
                else if (thisWeekRadioButton.IsChecked)
                {
                    ApplyWeekFilter();
                }
                else if (thisMonthRadioButton.IsChecked)
                {
                    ApplyMonthFilter();
                }
                else if (thisYearRadioButton.IsChecked)
                {
                    ApplyYearFilter();
                }
            }
        }

        private void ApplyTodayFilter()
        {
            DateTime today = DateTime.Today;

            var todayExpenses = _allExpenses
                .Where(expense => expense.ExpenseDate.Date == today)
                .OrderByDescending(expense => expense.ExpenseDate)
                .ToList();

            UpdateExpenseList(todayExpenses);
        }

        private void ApplyWeekFilter()
        {
            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek); // Start of the current week (Sunday)

            var weekExpenses = _allExpenses
                .Where(expense => expense.ExpenseDate.Date >= startOfWeek)
                .OrderByDescending(expense => expense.ExpenseDate)
                .ToList();

            UpdateExpenseList(weekExpenses);
        }

        private void ApplyMonthFilter()
        {
            DateTime today = DateTime.Today;
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1); // First day of the current month

            var monthExpenses = _allExpenses
                .Where(expense => expense.ExpenseDate.Date >= startOfMonth)
                .OrderByDescending(expense => expense.ExpenseDate)
                .ToList();

            UpdateExpenseList(monthExpenses);
        }

        private void ApplyYearFilter()
        {
            int currentYear = DateTime.Today.Year;

            var yearExpenses = _allExpenses
                .Where(expense => expense.ExpenseDate.Year == currentYear)
                .OrderByDescending(expense => expense.ExpenseDate)
                .ToList();

            UpdateExpenseList(yearExpenses);
        }

        // Helper function to update the ListView and UI
        private void UpdateExpenseList(List<Expense> filteredExpenses)
        {
            _expenses.Clear(); // Clears the current list
            foreach (var expense in filteredExpenses)
                _expenses.Add(expense); // Automatically updates the UI

            ExpenseListView.ItemsSource = _expenses;
            UpdateExpenseCount();
        }


    }
}
