using System.ComponentModel;
using Microcharts;
using SkiaSharp;
using DentalApp.Services;
using DentalApp.Services.ApiServices;
using DentalApp.Models;

namespace DentalApp.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly SaleService _saleService = new();
        private readonly ExpenseService _expenseService = new();
        private readonly DashboardService _dashboardService = new();
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
            var entries = new List<ChartEntry>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                decimal income = await _dashboardService.GetTotalIncomeAsync(startDate, endDate);
                decimal expense = await _dashboardService.GetTotalExpenseAsync(startDate, endDate);
                float netIncome = (float)(income - expense);

                entries.Add(new ChartEntry(netIncome)
                {
                    Label = startDate.ToString("MMM"),
                    ValueLabel = netIncome.ToString("N2"),
                    Color = SKColor.Parse("#2196F3")
                });
            }

            _updateRevenueChart?.Invoke(entries);
        }

        public async Task<List<ChartEntry>> LoadMonthlyRevenueExpenseBarChartAsync(int year)
        {
            var entries = new List<ChartEntry>();

            for (int month = 1; month <= 12; month++)
            {
                var startDate = new DateTime(year, month, 1);
                var endDate = startDate.AddMonths(1).AddDays(-1);

                decimal income = await _dashboardService.GetTotalIncomeAsync(startDate, endDate);
                decimal expense = await _dashboardService.GetTotalExpenseAsync(startDate, endDate);

                float revenue = (float)income;
                float exp = (float)expense;

                // Spacer
                entries.Add(new ChartEntry(0) { Label = "", ValueLabel = "", Color = SKColors.Transparent });

                entries.Add(new ChartEntry(revenue)
                {
                    Label = startDate.ToString("MMM"),
                    ValueLabel = revenue.ToString("N2"),
                    Color = SKColor.Parse("#00C853")
                });

                entries.Add(new ChartEntry(exp)
                {
                    Label = "",
                    ValueLabel = exp.ToString("N2"),
                    Color = SKColor.Parse("#D50000")
                });

                // Spacer
                entries.Add(new ChartEntry(0) { Label = "", ValueLabel = "", Color = SKColors.Transparent });
            }

            return entries;
        }

        public async Task LoadSalesExpenseAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            if (!startDate.HasValue || !endDate.HasValue)
            {
                var now = DateTime.Now;
                startDate = new DateTime(now.Year, 1, 1);
                endDate = new DateTime(now.Year, 12, 31);
            }

            var income = await _dashboardService.GetTotalIncomeAsync(startDate.Value, endDate.Value);
            var expense = await _dashboardService.GetTotalExpenseAsync(startDate.Value, endDate.Value);

            SalesValue = Convert.ToInt32(income);
            ExpensesValue = Convert.ToInt32(expense);

            OnPropertyChanged(nameof(SalesValue));
            OnPropertyChanged(nameof(ExpensesValue));

            _updateMainChart?.Invoke(SalesValue, ExpensesValue);

            var dentistEntries = await LoadDentistRevenueChartAsync(startDate, endDate);
            var expenseEntries = await LoadExpenseCategoryChartAsync(startDate, endDate);
            _updateSubCharts?.Invoke(dentistEntries, expenseEntries);
        }


        public async Task<List<ChartEntry>> LoadDentistRevenueChartAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var sales = await _saleService.GetSalesAsync();

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
                ValueLabel = g.Total.ToString("N2"),
                Color = SKColor.Parse("#4CAF50")
            }).ToList();
        }

        public async Task<List<ChartEntry>> LoadExpenseCategoryChartAsync(DateTime? startDate = null, DateTime? endDate = null)
        {
            var expenses = await _expenseService.GetExpensesAsync();

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
                ValueLabel = g.Total.ToString("N2"),
                Color = SKColor.Parse("#F44336")
            }).ToList();
        }

        public async Task<decimal> GetMonthlyIncome(DateTime start, DateTime end)
        {
            return await _dashboardService.GetTotalIncomeAsync(start, end);
        }
        public async Task<decimal> GetMonthlyExpense(DateTime start, DateTime end)
        {
            return await _dashboardService.GetTotalExpenseAsync(start, end);
        }

        protected void OnPropertyChanged(string name) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
