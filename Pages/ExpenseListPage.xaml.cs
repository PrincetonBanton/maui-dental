using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Services.Validations;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class ExpenseListPage : ContentPage
    {
        private readonly ExpenseService _expenseService = new();
        private ObservableCollection<Expense> _allExpenses = new();
        private ObservableCollection<Expense> _filteredExpenses = new();

        public ExpenseListPage()
        {
            InitializeComponent();
            LoadExpenseList();
        }
        //protected override void OnAppearing()
        //{
        //    base.OnAppearing();
        //    LoadExpenseList();
        //}
        private async void LoadExpenseList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                var expenseList = isApiAvailable
                    ? await _expenseService.GetExpensesAsync() ?? new List<Expense>()
                    : SampleData.GetSampleExpenses();

                _allExpenses.Clear();
                _filteredExpenses.Clear();

                expenseList.ForEach(e =>
                {
                    _allExpenses.Add(e);
                    _filteredExpenses.Add(e);
                });
                ExpenseListView.ItemsSource = _allExpenses;
                //ExpenseListView.ItemsSource = _filteredExpenses;
                UpdateExpenseCount();

            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load expenses. Please try again.", "OK");
            }
        }

        private async void OnCreateExpenseButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ExpenseDetailsPage(_allExpenses));
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Expense selectedExpense)
            {
                await Navigation.PushAsync(new ExpenseDetailsPage(_allExpenses, selectedExpense));
            }
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is Expense selectedExpense)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _expenseService.DeleteExpenseAsync(selectedExpense.Id);
                if (success)
                {
                    _allExpenses.Remove(selectedExpense);
                }

                await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
                UpdateExpenseCount();
            }
        }

        private void OnCustomDateCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            bool isChecked = customDateCheckBox.IsChecked;
            expenseStartPicker.IsEnabled = isChecked;
            expenseEndPicker.IsEnabled = isChecked;
            quickFilterCheckBox.IsChecked = !isChecked;
            todayRadioButton.IsEnabled = !isChecked;
            thisWeekRadioButton.IsEnabled = !isChecked;
            thisMonthRadioButton.IsEnabled = !isChecked;
            thisYearRadioButton.IsEnabled = !isChecked;
            if (isChecked) ApplyCustomDateFilter();
        }
        private void OnQuickFilterCheckedChanged(object sender, CheckedChangedEventArgs e)
        {
            bool isChecked = quickFilterCheckBox.IsChecked;
            todayRadioButton.IsEnabled = isChecked;
            thisWeekRadioButton.IsEnabled = isChecked;
            thisMonthRadioButton.IsEnabled = isChecked;
            thisYearRadioButton.IsEnabled = isChecked;
            customDateCheckBox.IsChecked = !isChecked;
            expenseStartPicker.IsEnabled = !isChecked;
            expenseEndPicker.IsEnabled = !isChecked;
        }
        private void OnQuickFilterRadioButtonChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!quickFilterCheckBox.IsChecked) return;

            DateTime today = DateTime.Today;
            DateTime startOfWeek = today.AddDays(-(int)today.DayOfWeek);
            DateTime startOfMonth = new DateTime(today.Year, today.Month, 1);
            int currentYear = today.Year;

            if (todayRadioButton.IsChecked)
                ApplyFilter(e => e.ExpenseDate.Date == today);
            else if (thisWeekRadioButton.IsChecked)
                ApplyFilter(e => e.ExpenseDate >= startOfWeek);
            else if (thisMonthRadioButton.IsChecked)
                ApplyFilter(e => e.ExpenseDate >= startOfMonth);
            else if (thisYearRadioButton.IsChecked)
                ApplyFilter(e => e.ExpenseDate.Year == currentYear);
        }
        private void ApplyFilter(Func<Expense, bool> filterCriteria)
        {
            var filtered = _allExpenses.Where(filterCriteria).ToList();
            _filteredExpenses.Clear();
            foreach (var e in filtered)
                _filteredExpenses.Add(e);
            ExpenseListView.ItemsSource = _filteredExpenses;
            UpdateExpenseCount();
        }
        private void ApplyCustomDateFilter()
        {
            if (!customDateCheckBox.IsChecked) return;

            DateTime startDate = expenseStartPicker.Date;
            DateTime endDate = expenseEndPicker.Date;
            ApplyFilter(e => e.ExpenseDate.Date >= startDate && e.ExpenseDate.Date <= endDate);
        }

        private void UpdateExpenseCount() => ExpenseCountLabel.Text = $"{_allExpenses.Count}";

        private void OnStartDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
        private void OnEndDateChanged(object sender, DateChangedEventArgs e) => ApplyCustomDateFilter();
        private void OnNumericEntryChanged(object sender, TextChangedEventArgs e) => NumericValidationService.OnNumericEntryChanged(sender, e);
    }
}
