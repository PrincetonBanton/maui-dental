using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services
{
    public static class DentistValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateDentist(DentistVM dentist)
        {
            if (string.IsNullOrWhiteSpace(dentist.FirstName))
                return (false, "First name is required.");
            if (string.IsNullOrWhiteSpace(dentist.MiddleName))
                //return (false, "Middle Name is required.");
                dentist.MiddleName = "-";
            if (string.IsNullOrWhiteSpace(dentist.LastName))
                return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(dentist.Address))
                return (false, "Address is required.");

            if (dentist.BirthDate == default)
                return (false, "A valid birth date is required.");

            if (!IsValidEmail(dentist.Email))
                return (false, "Invalid email format.");

            if (!IsValidMobileNumber(dentist.Mobile))
                return (false, "Invalid mobile number format.");

            return (true, string.Empty);
        }

        private static bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                return false;
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        private static bool IsValidMobileNumber(string mobile)
        {
            if (string.IsNullOrWhiteSpace(mobile))
                return false;
            var mobilePattern = @"^\d{10,15}$";
            return Regex.IsMatch(mobile, mobilePattern);
        }
    }
}
