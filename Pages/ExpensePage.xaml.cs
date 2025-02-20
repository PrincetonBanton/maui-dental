using DentalApp.Models;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class ExpensePage : ContentPage
    {
        private Expense _currentExpense = new();
        private bool _isInternetAvailable;
        private readonly ApiService _apiService = new();

        public ExpensePage(Expense expense = null)
        {
            InitializeComponent();
            CheckConnectivity();
           
            _currentExpense = expense ?? new Expense();
            BindExpenseToForm();
        }

        private async void CheckConnectivity()
        {
            await ConnectivityService.Instance.CheckAndUpdateConnectivityAsync();
            _isInternetAvailable = ConnectivityService.Instance.IsInternetAvailable;
            internetStatusLabel.Text = _isInternetAvailable ? "Online" : "Offline";
            internetStatusLabel.TextColor = _isInternetAvailable ? Colors.Green : Colors.Red;

            if (_isInternetAvailable)
            {
                //var localExpenses = await _databaseService.GetExpensesAsync();
                //if (localExpenses.Any()) await MigrateLocalDataToApi(localExpenses);
  
                LoadExpenses();
                //LoadOnlineData();
            }
            else
            {
                await DisplayAlert("Connectivity", "You are currently offline", "Ok");
                //LoadOfflineData();
            }
        }

        private async void LoadExpenses()
        {
            var expenses = await _apiService.GetExpensesAsync();
            var users = await _apiService.GetUsersAsync(); // Fetch users

            // Map EnteredBy to the corresponding Full Name from User
            foreach (var expense in expenses)
            {
                var user = users.FirstOrDefault(u => u.Id == expense.EnteredBy);
                expense.EnteredByName = user?.FullName ?? "Unknown"; // Fallback to "Unknown" if no user is found
            }

            expenseListView.ItemsSource = expenses;
            categoryPicker.ItemsSource = await _apiService.GetExpenseCategoryAsync();
        }

        private async void OnSaveButtonClicked(object sender, EventArgs e)
        {
            //if (!await ValidateExpenseInput())
            //    return;

            _currentExpense.Description = descriptionEntry.Text;
            _currentExpense.Amount = decimal.Parse(amountEntry.Text);
            _currentExpense.ExpenseDate = expenseDatePicker.Date;
            _currentExpense.EnteredBy = 1;  //Temporary
            _currentExpense.ExpenseCategoryId = ((ExpenseCategory)categoryPicker.SelectedItem).Id;

            await DisplayAlert("Confirm Expense",
                $"Description: {_currentExpense.Description}\n" +
                $"Amount: {_currentExpense.Amount:C}\n" +
                $"Date: {_currentExpense.ExpenseDate:MM/dd/yyyy}\n" +
                $"Category ID: {_currentExpense.ExpenseCategoryId}",
            "OK");

            var result = _currentExpense.Id == 0
                ? await _apiService.CreateExpenseAsync(_currentExpense)
                : await _apiService.UpdateExpenseAsync(_currentExpense);
            LoadExpenses();
            inputFrame.IsVisible = false;

            //if (_isInternetAvailable)
            //{
            //    var result = _currentExpense.ExpenseId == 0
            //        ? await _apiService.CreateExpenseAsync(_currentExpense)
            //        : await _apiService.UpdateExpenseAsync(_currentExpense);
            //    LoadOnlineData();
            //}
            //else
            //{
            //    var result = _currentExpense.ExpenseId == 0
            //        ? await _databaseService.SaveExpenseAsync(_currentExpense)
            //        : await _databaseService.UpdateExpenseAsync(_currentExpense);
            //    LoadOfflineData();
            //}

            await DisplayAlert("Success", "Expense saved.", "OK");
            ClearForm();
        }

        private async void ExpenseListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            if (e.Item is not Expense selectedExpense) return;

            string action = await DisplayActionSheet("Action", "Cancel", null, "Add","Edit", "Delete");

            if (action == "Add") { 
                inputFrame.IsVisible = true;
            }
            else if (action == "Edit")
            {
                inputFrame.IsVisible = true;
                _currentExpense = selectedExpense;
                BindExpenseToForm();
            }
            else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No"))
            {
                if (_isInternetAvailable)
                {
                    var success = await _apiService.DeleteExpenseAsync(selectedExpense.Id);
                    await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
                    LoadExpenses();
                    //LoadOnlineData();
                }
                else
                {
                    //var success = await _databaseService.DeleteExpenseAsync(selectedExpense.ExpenseId) > 0;
                    //LoadOfflineData();
                    //await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted offline." : "Failed to delete expense offline.", "OK");
                }
            }
             ((ListView)sender).SelectedItem = null;
        }

        private void BindExpenseToForm()
        {
            descriptionEntry.Text = _currentExpense.Description;
            amountEntry.Text = _currentExpense.Amount > 0 ? _currentExpense.Amount.ToString() : string.Empty;
            expenseDatePicker.Date = _currentExpense.ExpenseDate != default ? _currentExpense.ExpenseDate : DateTime.Now;
            categoryPicker.SelectedItem = (categoryPicker.ItemsSource as List<ExpenseCategory>)?
                .FirstOrDefault(c => c.Id == _currentExpense.ExpenseCategoryId);
        }
        private void ClearForm()
        {
            _currentExpense = new Expense();
            descriptionEntry.Text = string.Empty;
            amountEntry.Text = string.Empty;
            expenseDatePicker.Date = DateTime.Now;
            categoryPicker.SelectedItem = null;
        }
    }
}

