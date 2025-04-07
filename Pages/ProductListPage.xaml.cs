using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
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
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load products. Please try again.", "OK");
            }
            ProductListView.ItemsSource = _allProducts;

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
        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();

            ProductListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allProducts
                : _allProducts.Where(p => p.Name.ToLower().Contains(searchText)).ToList();

        }
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}
