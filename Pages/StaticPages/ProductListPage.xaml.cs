using DentalApp.Models;

namespace DentalApp.Pages.StaticPages
{
    public partial class ProductListPage : ContentPage
    {
        public ProductListPage()
        {
            InitializeComponent();
            LoadSampleData();
        }

        private void LoadSampleData()
        {
            var sampleUsers = new List<ProductVM>
            {
                new ProductVM { Name = "ProductTest"},
                new ProductVM { Name = "ProductTest"},
                new ProductVM { Name = "ProductTest"},
            };

            ProductListView.ItemsSource = sampleUsers;
        }
    }
}
