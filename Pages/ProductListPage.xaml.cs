using DentalApp.Models;
using DentalApp.Services;
using DentalApp.Data;
using DentalApp.Models.Enum;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class ProductListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<ProductVM> _allProducts = new();
        private ObservableCollection<ProductVM> _filteredProducts = new();

        public ProductListPage()
        {
            InitializeComponent();
            ProductListView.ItemsSource = _filteredProducts;
            LoadProductList();
        }

        private async void LoadProductList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                _allProducts = isApiAvailable
                    ? await _apiService.GetProductsAsync() ?? new List<ProductVM>()
                    : SampleData.GetSampleProducts();               //Replace w offline data sync

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
                    : _allProducts.Where(p => p.ProductType == (selectedCategory == "Goods" ? ProductType.Goods : ProductType.Services));

                foreach (var product in filtered)
                {
                    _filteredProducts.Add(product);
                }
            }
        }

        private async void OnCreateProductButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductDetailsPage());
        }
        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is ProductVM selectedProduct)
            {
                await Navigation.PushAsync(new ProductDetailsPage(selectedProduct));
            }
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is ProductVM selectedProduct)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this product?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeleteProductAsync(selectedProduct.Id);
                LoadProductList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Product deleted." : "Failed to delete product.", "OK");
            }
        }

        private void OnCategoryChanged(object sender, EventArgs e) => FilterProducts();
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private void OnDropListImageTapped(object sender, TappedEventArgs e) => CategoryPicker.Focus();
    }
}
