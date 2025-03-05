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
                    _allProducts = await _apiService.GetProductsAsync() ?? new List<ProductVM>();
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
                    : _allProducts.Where(p => p.ProductType == (selectedCategory == "Products" ? ProductType.Goods : ProductType.Services));

                foreach (var product in filtered)
                {
                    _filteredProducts.Add(product);
                }
            }
        }

        private async void ProductListView_ItemTapped(object sender, ItemTappedEventArgs e)
        {
            //if (e.Item is not Expense selectedExpense) return;

            //string action = await DisplayActionSheet("Action", "Cancel", null, "Add", "Edit", "Delete");

            //if (action == "Add")
            //{
            //    inputFrame.IsVisible = true;
            //}
            //else if (action == "Edit")
            //{
            //    inputFrame.IsVisible = true;
            //    _currentExpense = selectedExpense;
            //    BindExpenseToForm();
            //}
            //else if (action == "Delete" && await DisplayAlert("Confirm", "Delete this expense?", "Yes", "No"))
            //{
            //    if (_isInternetAvailable)
            //    {
            //        var success = await _apiService.DeleteExpenseAsync(selectedExpense.Id);
            //        await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted." : "Failed to delete expense.", "OK");
            //        LoadExpenses();
            //        //LoadOnlineData();
            //    }
            //    else
            //    {
            //        //var success = await _databaseService.DeleteExpenseAsync(selectedExpense.ExpenseId) > 0;
            //        //LoadOfflineData();
            //        //await DisplayAlert(success ? "Success" : "Error", success ? "Expense deleted offline." : "Failed to delete expense offline.", "OK");
            //    }
            //}
            //((ListView)sender).SelectedItem = null;
        }

        private void OnCategoryChanged(object sender, EventArgs e) => FilterProducts();
        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
        private void OnDropListImageTapped(object sender, TappedEventArgs e) => CategoryPicker.Focus();

        private async void OnCreateProductButtonClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new ProductDetailsPage());
        }
        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            if (sender is ImageButton button && button.BindingContext is ProductVM selectedProduct)
            {
                bool confirmDelete = await DisplayAlert("Confirm", "Delete this patient?", "Yes", "No");
                if (!confirmDelete) return;

                var success = await _apiService.DeleteProductAsync(selectedProduct.Id);
                LoadProductList();
                await DisplayAlert(success ? "Success" : "Error", success ? "Patient deleted." : "Failed to delete patient.", "OK");
            }
        }
    }
}
