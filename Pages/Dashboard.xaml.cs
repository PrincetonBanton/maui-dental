using DentalApp.Services;
using Microcharts;
using SkiaSharp;
using Microcharts.Maui;
using System.ComponentModel;

namespace DentalApp.Pages
{
    public partial class Dashboard : ContentPage
    {
        private DashboardViewModel _viewModel;

        public Dashboard()
        {
            InitializeComponent();
            _viewModel = new DashboardViewModel(UpdateCharts);
            BindingContext = _viewModel;
        }

        //protected override async void OnAppearing()
        //{
        //    base.OnAppearing();

        //    var tokenJson = Preferences.Get("AuthToken", string.Empty);
        //    if (string.IsNullOrEmpty(tokenJson) || TokenService.IsTokenExpired(tokenJson))
        //    {
        //        Preferences.Remove("AuthToken");
        //        Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
        //    }
        //    else
        //    {
        //        await _viewModel.LoadDataAsync();
        //    }
        //}

        private void UpdateCharts(int salesValue, int expensesValue)
        {
            var barEntries = new[]
            {
                new ChartEntry(salesValue) { Label = "Sales", ValueLabel = salesValue.ToString(), Color = SKColor.Parse("#00C853") },
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
                new ChartEntry(salesValue) { Color = SKColor.Parse("#00C853") },
                new ChartEntry(expensesValue) { Color = SKColor.Parse("#D50000") }
            };

            SalesExpensePieChart.Chart = new DonutChart
            {
                Entries = pieEntries,
                LabelTextSize = 20,
                HoleRadius = 0.4f
            };
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
                // Pass null to load everything
                await _viewModel.LoadDataAsync();
                return;
            }

            await _viewModel.LoadDataAsync(startDate, endDate);
        }


        public class DashboardViewModel : INotifyPropertyChanged
        {
            private readonly ApiService _apiService = new();
            private readonly Action<int, int> _updateChartAction;

            public int SalesValue { get; set; }
            public int ExpensesValue { get; set; }

            public event PropertyChangedEventHandler PropertyChanged;

            public DashboardViewModel(Action<int, int> updateChart)
            {
                _updateChartAction = updateChart;
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

                SalesValue = (int)(sales?.Sum(s => s.Total) ?? 0);
                ExpensesValue = (int)(expenses?.Sum(e => e.Amount) ?? 0);

                OnPropertyChanged(nameof(SalesValue));
                OnPropertyChanged(nameof(ExpensesValue));

                _updateChartAction?.Invoke(SalesValue, ExpensesValue);
            }

            protected void OnPropertyChanged(string name) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        //--------- CYCLE PRESENTATION COMMENT OUT --------------
        protected override async void OnAppearing()
        {
            base.OnAppearing();
            StartCyclingRadioButtons();
            var tokenJson = Preferences.Get("AuthToken", string.Empty);
            if (string.IsNullOrEmpty(tokenJson) || TokenService.IsTokenExpired(tokenJson))
            {
                Preferences.Remove("AuthToken");
                Application.Current.MainPage = new NavigationPage(new Pages.Auth.LoginPage());
            }
            else
            {
                await _viewModel.LoadDataAsync();
            }
        }

        protected override void OnDisappearing()
        {
            base.OnDisappearing();
            StopCyclingRadioButtons();
        }
        private CancellationTokenSource _radioCycleCts;
        private void StartCyclingRadioButtons()
        {
            _radioCycleCts = new CancellationTokenSource();
            _ = Task.Run(async () =>
            {
                var buttons = new[] { todayRadioButton, thisWeekRadioButton, thisMonthRadioButton, thisYearRadioButton, allTimeRadioButton };
                int index = 0;

                while (!_radioCycleCts.IsCancellationRequested)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        buttons[index].IsChecked = true;
                    });

                    index = (index + 1) % buttons.Length;
                    await Task.Delay(3000); // 3 seconds
                }
            });
        }
        private void StopCyclingRadioButtons()
        {
            _radioCycleCts?.Cancel();
        }
    }
}
