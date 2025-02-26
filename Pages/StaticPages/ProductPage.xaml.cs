using Microsoft.Maui.Controls;
using System.Collections.Generic;

namespace DentalApp.Pages.StaticPages
{
    public partial class ProductPage : ContentPage
    {
        public ProductPage()
        {
            InitializeComponent();

            // Create a list of sample products
            var products = new List<Product>
            {
                new Product { Name = "Product 1", Description = "Description 1", ImageSource = "default_product_image.png" },
                new Product { Name = "Product 2", Description = "Description 2", ImageSource = "default_product_image.png" },
                new Product { Name = "Product 1", Description = "Description 1", ImageSource = "default_product_image.png" },
                new Product { Name = "Product 2", Description = "Description 2", ImageSource = "default_product_image.png" },
                // Add more products as needed
            };

            // Set the ItemsSource of the CollectionView
            ProductCollectionView.ItemsSource = products;
        }
    }

    // Define the Product class
    public class Product
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string ImageSource { get; set; }
    }
}
