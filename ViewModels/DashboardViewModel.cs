using System.ComponentModel;
using Microcharts;
using SkiaSharp;
using DentalApp.Services;
using DentalApp.Models;

namespace DentalApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();
        private readonly Action<List<ChartEntry>> _updateRevenueChart;
        private readonly Action<int, int> _updateMainChart;
        private readonly Action<List<ChartEntry>, List<ChartEntry>> _updateSubCharts;

        public int SalesValue { get; set; }
        public int ExpensesValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel(
            Action<List<ChartEntry>> updateRevenueChart,
            Action<int, int> updateMainChart,
            Action<List<ChartEntry>, List<ChartEntry>> updateSubCharts)
        {
            _updateRevenueChart = updateRevenueChart;
            _updateMainChart = updateMainChart;
            _updateSubCharts = updateSubCharts;
        }

        public async Task LoadMonthlyRevenueChartAsync(int year)
        {
            var sales = await _apiService.GetSalesAsync();
            var expenses = await _apiService.GetExpensesAsync();

            var monthlySales = sales?
                .Where(s => s.SaleDate.Year == year)
                .GroupBy(s => s.SaleDate.Month)
                .ToDictionary(g => g.Key, g => (float)g.Sum(s => s.Total)) ?? new();

            var monthlyExpenses = expenses?
                .Where(e => e.ExpenseDate.Year == year)
                .GroupBy(e => e.ExpenseDate.Month)
                .ToDictionary(g => g.Key, g => (float)g.Sum(e => e.Amount)) ?? new();

            var entries = new List<ChartEntry>();

            for (int month = 1; month <= 12; month++)
            {
                float revenue = monthlySales.TryGetValue(month, out var rev) ? rev : 0f;
                float expense = monthlyExpenses.TryGetValue(month, out var exp) ? exp : 0f;
                float income = revenue - expense;

                entries.Add(new ChartEntry(income)
                {
                    Label = new DateTime(year, month, 1).ToString("MMM"),
                    ValueLabel = income.ToString("0"),
                    Color = SKColor.Parse("#2196F3") // You can change color if you want to distinguish income
                });
            }

            _updateRevenueChart?.Invoke(entries);
        }


        public async Task<List<ChartEntry>> LoadMonthlyRevenueExpenseBarChartAsync(int year)
        {
            var sales = await _apiService.GetSalesAsync();
            var expenses = await _apiService.GetExpensesAsync();

            var monthlySales = sales?
                .Where(s => s.SaleDate.Year == year)
                .GroupBy(s => s.SaleDate.Month)
                .ToDictionary(g => g.Key, g => (float)g.Sum(s => s.Total)) ?? new();

            var monthlyExpenses = expenses?
                .Where(e => e.ExpenseDate.Year == year)
                .GroupBy(e => e.ExpenseDate.Month)
                .ToDictionary(g => g.Key, g => (float)g.Sum(e => e.Amount)) ?? new();

            var entries = new List<ChartEntry>();

            for (int month = 1; month <= 12; month++)
            {
                float revenue = monthlySales.TryGetValue(month, out var rev) ? rev : 0f;
                float expense = monthlyExpenses.TryGetValue(month, out var exp) ? exp : 0f;

                //Spacer
                entries.Add(new ChartEntry(0)
                {
                    Label = "",
                    ValueLabel = "",
                    Color = SKColors.Transparent
                });
                entries.Add(new ChartEntry(revenue)
                {
                    Label = new DateTime(year, month, 1).ToString("MMM"),
                    ValueLabel = revenue.ToString("0"),
                    Color = SKColor.Parse("#00C853") // Green for revenue
                });

                entries.Add(new ChartEntry(expense)
                {
                    Label = "", // No duplicate month label
                    ValueLabel = expense.ToString("0"),
                    Color = SKColor.Parse("#D50000") // Red for expense
                });
                //Spacer
                entries.Add(new ChartEntry(0)
                {
                    Label = "",
                    ValueLabel = "",
                    Color = SKColors.Transparent
                });
            }

            return entries;
        }

        public async Task LoadSalesExpenseAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sales = await _apiService.GetSalesAsync();
            var expenses = await _apiService.GetExpensesAsync();

            if (startDate.HasValue && endDate.HasValue)
            {
                sales = sales?.Where(s => s.SaleDate.Date >= startDate.Value && s.SaleDate.Date <= endDate.Value).ToList();
                expenses = expenses?.Where(e => e.ExpenseDate.Date >= startDate.Value && e.ExpenseDate.Date <= endDate.Value).ToList();
            }

            SalesValue = Convert.ToInt32(sales?.Sum(s => s.Total) ?? 0m);
            ExpensesValue = Convert.ToInt32(expenses?.Sum(e => e.Amount) ?? 0m);

            OnPropertyChanged(nameof(SalesValue));
            OnPropertyChanged(nameof(ExpensesValue));

            _updateMainChart?.Invoke(SalesValue, ExpensesValue);

            var dentistEntries = await LoadDentistRevenueChartAsync(startDate, endDate);
            var expenseEntries = await LoadExpenseCategoryChartAsync(startDate, endDate);
            _updateSubCharts?.Invoke(dentistEntries, expenseEntries);
        }

        public async Task<List<ChartEntry>> LoadDentistRevenueChartAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sales = await _apiService.GetSalesAsync();

            if (startDate.HasValue && endDate.HasValue)
                sales = sales?.Where(s => s.SaleDate.Date >= startDate.Value && s.SaleDate.Date <= endDate.Value).ToList();

            if (sales?.Any() != true)
                return [new ChartEntry(0) { Label = "N/A", ValueLabel = "0", Color = SKColor.Parse("#4CAF50") }];

            var grouped = sales
                .GroupBy(s => s.DentistName)
                .Select(g => new
                {
                    Dentist = g.Key,
                    Total = (float)g.Sum(e => e.Total)
                }).ToList();

            return grouped.Select(g => new ChartEntry(g.Total)
            {
                Label = g.Dentist,
                ValueLabel = g.Total.ToString("0"),
                Color = SKColor.Parse("#4CAF50")
            }).ToList();
        }

        public async Task<List<ChartEntry>> LoadExpenseCategoryChartAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var expenses = await _apiService.GetExpensesAsync();

            if (startDate.HasValue && endDate.HasValue)
                expenses = expenses?.Where(e => e.ExpenseDate.Date >= startDate.Value && e.ExpenseDate.Date <= endDate.Value).ToList();

            if (expenses?.Any() != true)
                return [new ChartEntry(0) { Label = "N/A", ValueLabel = "0", Color = SKColor.Parse("#F44336") }];

            var grouped = expenses
                .Where(e => e.ExpenseCategory != null)
                .GroupBy(e => e.ExpenseCategory.Name)
                .Select(g => new
                {
                    CategoryName = g.Key,
                    Total = (float)g.Sum(e => e.Amount)
                }).ToList();

            return grouped.Select(g => new ChartEntry(g.Total)
            {
                Label = g.CategoryName,
                ValueLabel = g.Total.ToString("0"),
                Color = SKColor.Parse("#F44336")
            }).ToList();
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
