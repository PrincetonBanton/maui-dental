using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services
{
    public static class ProductValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateProduct(ProductVM product)
        {
            if (string.IsNullOrWhiteSpace(product.Name))
                return (false, "Product name is required.");

            if (string.IsNullOrWhiteSpace(product.ProductCode))
                return (false, "Product code is required.");

            if (string.IsNullOrWhiteSpace(product.Description))
                return (false, "Description is required.");

            if (product.Amount <= 0)
                return (false, "Amount must be a positive number.");

            if (product.MinPrice <= 0)
                return (false, "Minimum price must be a positive number.");

            if (product.MaxPrice <= 0)
                return (false, "Maximum price must be a positive number.");

            if (product.MinPrice > product.MaxPrice)
                return (false, "Minimum price cannot be greater than maximum price.");

            return (true, string.Empty);
        }
    }
}
