using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Data;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class ProductPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<Product> _allProducts = new();
        private ObservableCollection<Product> _filteredProducts = new();

        public ProductPage()
        {
            InitializeComponent();
            ProductCollectionView.ItemsSource = _filteredProducts;
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

                // Display all products initially (unfiltered)
                _filteredProducts.Clear();
                foreach (var product in _allProducts)
                {
                    _filteredProducts.Add(product);
                }
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
            }
        }

        private void FilterProducts()
        {
            if (CategoryPicker.SelectedItem is string selectedCategory)
            {
                _filteredProducts.Clear();
                var filtered = selectedCategory == "All"
                    ? _allProducts
                    : _allProducts.Where(p => p.ProductType == (selectedCategory == "Products" ? 1 : 2));

                foreach (var product in filtered)
                {
                    _filteredProducts.Add(product);
                }
            }
        }


        private void OnCategoryChanged(object sender, EventArgs e) => FilterProducts();

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private void OnDropListImageTapped(object sender, TappedEventArgs e) => CategoryPicker.Focus();
    }
}
