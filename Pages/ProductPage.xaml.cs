using DentalApp.Models;
using DentalApp.Data;
using DentalApp.Services;

namespace DentalApp.Pages
{
    public partial class ProductPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<Product> _allProducts = new();

        public ProductPage()
        {
            InitializeComponent();
            LoadProductList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            LoadProductList();
        }

        private async void LoadProductList()
        {
            try
            {
                if (Connectivity.NetworkAccess == NetworkAccess.Internet)
                {
                    _allProducts = await _apiService.GetProductsAsync() ?? new List<Product>();
                }
                else
                {
                    _allProducts = SampleData.GetSampleProducts();  
                }
                ProductCollectionView.ItemsSource = _allProducts;
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
            }
        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private void OnDropListImageTapped(object sender, TappedEventArgs e) => CategoryPicker.Focus();

    }
}
