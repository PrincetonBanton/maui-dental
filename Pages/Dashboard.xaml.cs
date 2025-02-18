using Microcharts;
using SkiaSharp;

namespace DentalApp.Pages
{
    public partial class Dashboard : ContentPage
    {
        public Dashboard()
        {
            InitializeComponent();
            LoadChartDataAsync(); // Automatically load data when the page initializes
        }

        private async Task<List<float>> FetchRevenueDataAsync()
        {
            await Task.Delay(1000); // Simulate API delay
            return new List<float> { 500, 300, 400 }; // Example data for Jan, Feb, Mar
        }

        private async void LoadChartDataAsync()
        {
            List<float> revenueDataFromApi = await FetchRevenueDataAsync();
            var revenueData = new List<ChartEntry>();

            string[] months = { "Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec" };

            for (int i = 0; i < 12; i++)
            {
                float revenueValue = i < revenueDataFromApi.Count ? revenueDataFromApi[i] : 0;
                revenueData.Add(new ChartEntry(revenueValue)
                {
                    Label = months[i],
                    ValueLabel = revenueValue.ToString(),
                    Color = SKColor.Parse(GetColorForMonth(i))
                });
            }

            float maxValue = revenueData.Any() ? (revenueData.Max(entry => entry.Value) ?? 0) * 1.2f : 100;

            RevenueChart.Chart = new BarChart
            {
                Entries = revenueData,
                LabelTextSize = 20,
                BackgroundColor = SKColors.White,
                MaxValue = maxValue,
                LabelOrientation = Microcharts.Orientation.Horizontal
            };
        }

        private string GetColorForMonth(int monthIndex)
        {
            string[] colors = { "#266489", "#68B9C0", "#90D585", "#F3C151", "#F28384", "#89B4C6", "#D3F1F0", "#F7E4C7", "#9AB9D7", "#67C1A3", "#F1C3A5", "#D0B2F7" };
            return colors[monthIndex % colors.Length];
        }
    }
}
