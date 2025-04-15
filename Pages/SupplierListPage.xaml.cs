using DentalApp.Data;
using DentalApp.Models;
using DentalApp.Services;
using System.Collections.ObjectModel;

namespace DentalApp.Pages
{
    public partial class SupplierListPage : ContentPage
    {
        private readonly ApiService _apiService = new();
        private List<Supplier> _allSuppliers = new();
        private ObservableCollection<Supplier> _filteredSuppliers = new();
        private bool _isLandscape = false;

        public SupplierListPage()
        {
            InitializeComponent();
            LoadSupplierList();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();
            UpdateOrientation(Width, Height); // Check orientation on initial page load
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);
            UpdateOrientation(width, height); // Also check when size changes
        }

        private void UpdateOrientation(double width, double height)
        {
            bool newIsLandscape = width > height;
            _isLandscape = newIsLandscape;

            var templateKey = _isLandscape ? "LandscapeTemplate" : "PortraitTemplate";
            var template = (DataTemplate)this.Resources[templateKey];
            SupplierListView.ItemTemplate = template;

            //MainThread.BeginInvokeOnMainThread(async () =>
            //{
            //    await DisplayAlert("Orientation Changed", _isLandscape ? "Landscape" : "Portrait", "OK");
            //});
        }


        private async void LoadSupplierList()
        {
            await ApiConnectivityService.Instance.CheckApiConnectivityAsync();
            bool isApiAvailable = ApiConnectivityService.Instance.IsApiAvailable;
            try
            {
                _allSuppliers = isApiAvailable
                    ? await _apiService.GetSuppliersAsync() ?? new List<Supplier>()
                    : SampleData.GetSampleSuppliers(); // Replace with offline data sync
            }
            catch (Exception ex)
            {
                await DisplayAlert("Error", "Failed to load suppliers. Please try again.", "OK");
            }

            SupplierListView.ItemsSource = _allSuppliers;
        }

        private async void OnCreateSupplierButtonClicked(object sender, EventArgs e)
        {
            //await Navigation.PushAsync(new SupplierDetailsPage());
        }

        private async void OnEditButtonClicked(object sender, EventArgs e)
        {
            //if (sender is ImageButton button && button.BindingContext is SupplierVM selectedSupplier)
            //    await Navigation.PushAsync(new SupplierDetailsPage(selectedSupplier));
        }

        private async void OnDeleteButtonClicked(object sender, EventArgs e)
        {
            //if (sender is ImageButton button && button.BindingContext is SupplierVM selectedSupplier)
            //{
            //    bool confirmDelete = await DisplayAlert("Confirm", "Delete this supplier?", "Yes", "No");
            //    if (!confirmDelete) return;

            //    var success = await _apiService.DeleteSupplierAsync(selectedSupplier.Id);
            //    LoadSupplierList();
            //    await DisplayAlert(success ? "Success" : "Error", success ? "Supplier deleted." : "Failed to delete supplier.", "OK");
            //}
        }

        private void OnSearchBarTextChanged(object sender, TextChangedEventArgs e)
        {
            var searchText = e.NewTextValue.ToLower();

            SupplierListView.ItemsSource = string.IsNullOrWhiteSpace(searchText)
                ? _allSuppliers
                : _allSuppliers.Where(s => s.Name.ToLower().Contains(searchText)).ToList();
        }

        private void OnSearchImageTapped(object sender, TappedEventArgs e) => SearchBar.Focus();
    }
}
