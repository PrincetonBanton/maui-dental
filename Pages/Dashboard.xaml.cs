using DentalApp.Services;
using Microcharts;
using SkiaSharp;
using Microcharts.Maui;
using System.ComponentModel;
using DentalApp.Models;

namespace DentalApp.Pages
{
    public partial class Dashboard : ContentPage
    {
        private DashboardViewModel _viewModel;

        public Dashboard()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel(UpdateRevenueChart, UpdateMainCharts, UpdateSubCharts);
            BindingContext = _viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();

            var tokenJson = Preferences.Get("AuthToken", string.Empty);
            if (string.IsNullOrEmpty(tokenJson) || TokenService.IsTokenExpired(tokenJson))
            {
                Preferences.Remove("AuthToken");
                Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
            }
            else
            {
                await _viewModel.LoadDataAsync();
                await _viewModel.LoadMonthlyRevenueChartAsync(DateTime.Today.Year);
            }
        }

        private void OnLogoutClicked(object sender, EventArgs e)
        {
            Preferences.Remove("AuthToken");
            Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        }

        private async void OnQuickFilterRadioButtonChanged(object sender, CheckedChangedEventArgs e)
        {
            if (!e.Value) return;

            DateTime today = DateTime.Today;
            DateTime startDate = today;
            DateTime endDate = today;

            if (todayRadioButton.IsChecked)
            {
                startDate = today;
                endDate = today;
            }
            else if (thisWeekRadioButton.IsChecked)
            {
                int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
                startDate = today.AddDays(-1 * diff);
                endDate = today;
            }
            else if (thisMonthRadioButton.IsChecked)
            {
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = today;
            }
            else if (thisYearRadioButton.IsChecked)
            {
                startDate = new DateTime(today.Year, 1, 1);
                endDate = today;
            }
            else if (allTimeRadioButton.IsChecked)
            {
                await _viewModel.LoadDataAsync();
                return;
            }

            await _viewModel.LoadDataAsync(startDate, endDate);
        }
        private void UpdateRevenueChart(List<ChartEntry> monthlyEntries)
        {
            RevenueMonthlyBar.Chart = new LineChart
            {
                Entries = monthlyEntries,
                LabelTextSize = 14,
                LineMode = LineMode.Straight,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };
        }
        private void UpdateMainCharts(int salesValue, int expensesValue)
        {
            var barEntries = new[]
            {
                new ChartEntry(salesValue) { Label = "Revenues", ValueLabel = salesValue.ToString(), Color = SKColor.Parse("#00C853") },
                new ChartEntry(expensesValue) { Label = "Expenses", ValueLabel = expensesValue.ToString(),  Color = SKColor.Parse("#D50000") }
            };

            SalesExpenseChart.Chart = new BarChart
            {
                Entries = barEntries,
                LabelTextSize = 15,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };

            var pieEntries = new[]
            {
                new ChartEntry(salesValue) { Label = "Revenues", ValueLabel = salesValue.ToString(), Color = SKColor.Parse("#00C853") },
                new ChartEntry(expensesValue) { Label = "Expenses", ValueLabel = expensesValue.ToString(), Color = SKColor.Parse("#D50000") }
            };

            SalesExpensePieChart.Chart = new DonutChart
            {
                Entries = pieEntries,
                LabelTextSize = 20,
                LabelMode = LabelMode.None,
                HoleRadius = 0.4f
            };
            SalesValueLabel.Text = salesValue.ToString("N2");
            ExpensesValueLabel.Text = expensesValue.ToString("N2");
            IncomeValueLabel.Text = (salesValue - expensesValue).ToString("N2");
        }

        private void UpdateSubCharts(List<ChartEntry> dentistEntries, List<ChartEntry> expenseEntries)
        {
            DentistSaleChart.Chart = new BarChart
            {
                Entries = dentistEntries,
                LabelTextSize = 14,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };

            ExpenseCategoryChart.Chart = new BarChart
            {
                Entries = expenseEntries,
                LabelTextSize = 14,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };
        }
    }

    // DashboardViewModel included temporarily in the same file
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly ApiService _apiService = new();
        private readonly Action<List<ChartEntry>> _updateRevenueChart;
        private readonly Action<int, int> _updateMainChart;
        private readonly Action<List<ChartEntry>, List<ChartEntry>> _updateSubCharts;

        public int SalesValue { get; set; }
        public int ExpensesValue { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;

        public DashboardViewModel(Action<List<ChartEntry>> updateRevenueChart, Action<int, int> updateMainChart, Action<List<ChartEntry>, List<ChartEntry>> updateSubCharts)
        {
            _updateRevenueChart = updateRevenueChart;
            _updateMainChart = updateMainChart;
            _updateSubCharts = updateSubCharts;
        }

        public async Task LoadMonthlyRevenueChartAsync(int year)
        {
            var sales = await _apiService.GetSalesAsync();

            var monthlyTotals = sales?
                .Where(s => s.SaleDate.Year == year)
                .GroupBy(s => s.SaleDate.Month)
                .ToDictionary(g => g.Key, g => (float)g.Sum(s => s.Total)) ?? new Dictionary<int, float>();

            var entries = new List<ChartEntry>();

            for (int month = 1; month <= 12; month++)
            {
                monthlyTotals.TryGetValue(month, out float total);

                entries.Add(new ChartEntry(total)
                {
                    Label = new DateTime(year, month, 1).ToString("MMM"),
                    ValueLabel = total.ToString("0"),
                    Color = SKColor.Parse("#2196F3")
                });
            }

            _updateRevenueChart?.Invoke(entries);
        }

        public async Task LoadDataAsync(DateTime? startDate = null, DateTime? endDate = null)
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

