using DentalApp.Services;
using Microcharts;
using SkiaSharp;
using Microcharts.Maui;
using System.ComponentModel;
using DentalApp.ViewModels;
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
                int currentYear = DateTime.Today.Year;
                await _viewModel.LoadMonthlyRevenueChartAsync(currentYear);
                var barEntries = await _viewModel.LoadMonthlyRevenueExpenseBarChartAsync(currentYear);
                UpdateMonthlyRevenueExpenseBarChart(barEntries);
                await _viewModel.LoadSalesExpenseAsync();
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
                await _viewModel.LoadSalesExpenseAsync();
                return;
            }

            await _viewModel.LoadSalesExpenseAsync(startDate, endDate);
        }

        private void UpdateRevenueChart(List<ChartEntry> monthlyEntries)
        {
            RevenueMonthlyBar.Chart = new LineChart
            {
                Entries = monthlyEntries,
                LabelTextSize = 13,
                LineMode = LineMode.Straight,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };
        }

        private void UpdateMonthlyRevenueExpenseBarChart(List<ChartEntry> entries)
        {
            RevenueExpenseMonthlyBarChart.Chart = new BarChart
            {
                Entries = entries,
                LabelTextSize = 11,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal,
                Margin = 5, // Increase space between bars
                BarAreaAlpha = 50 // Make bars fully opaque
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
                LabelTextSize = 13,
                ValueLabelOrientation = Orientation.Horizontal,
                //LabelOrientation = Orientation.Horizontal
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
                LabelTextSize = 13,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };

            ExpenseCategoryChart.Chart = new BarChart
            {
                Entries = expenseEntries,
                LabelTextSize = 13,
                ValueLabelOrientation = Orientation.Horizontal,
                LabelOrientation = Orientation.Horizontal
            };
        }
    }
}
