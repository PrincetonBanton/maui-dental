using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services.Validations
{
    public static class SupplierValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateSupplier(Supplier supplier)
        {
            if (string.IsNullOrWhiteSpace(supplier.Name))
                return (false, "Supplier name is required.");

            //if (string.IsNullOrWhiteSpace(supplier.FirstName))
            //    return (false, "First name is required.");

            //if (string.IsNullOrWhiteSpace(supplier.LastName))
            //    return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(supplier.Address))
                return (false, "Address is required.");

            if (!IsValidMobileNumber(supplier.Mobile))
                return (false, "Invalid mobile number format.");

            if (!string.IsNullOrWhiteSpace(supplier.Phone) && !IsValidPhoneNumber(supplier.Phone))
                return (false, "Invalid phone number format.");

            return (true, string.Empty);
        }

        private static bool IsValidMobileNumber(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return false;
            var mobilePattern = @"^\d{10,15}$";
            return Regex.IsMatch(mobile, mobilePattern);
        }

        private static bool IsValidPhoneNumber(string phone)
        {
            var phonePattern = @"^\d{7,15}$";
            return Regex.IsMatch(phone, phonePattern);
        }
    }
}
