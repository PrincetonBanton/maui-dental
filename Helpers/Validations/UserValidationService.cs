using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services.Validations
{
    public class UserValidationService
        {
        public static (bool IsValid, string ErrorMessage) ValidateUser(UserVM user, string confirmPassword, bool isEditMode = false)
        {
            //if (string.IsNullOrWhiteSpace(user.Username))
            //    return (false, "Username is required.");

            if (!isEditMode) // Only validate password if not editing
            {
                if (string.IsNullOrWhiteSpace(user.Password))
                    return (false, "Password is required.");

                if (string.IsNullOrWhiteSpace(confirmPassword))
                    return (false, "Password confirmation is required.");

                if (user.Password != confirmPassword)
                    return (false, "Passwords do not match.");
            }

            if (string.IsNullOrWhiteSpace(user.FirstName))
                return (false, "First name is required.");

            if (string.IsNullOrWhiteSpace(user.LastName))
                return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(user.MiddleName))
                user.MiddleName = "-"; // Default value

            if (string.IsNullOrWhiteSpace(user.Address))
                return (false, "Address is required.");

            if (user.BirthDate == default)
                return (false, "A valid birth date is required.");

            if (!IsValidEmail(user.Email))
                return (false, "Invalid email format.");

            if (!IsValidMobileNumber(user.Mobile))
                return (false, "Invalid mobile number format.");

            if (user.RoleId == 0 && !isEditMode)
                return (false, "Please select a valid role.");

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
                var mobilePattern = @"^\d{10,15}$"; // Allows 10-15 digits
                return Regex.IsMatch(mobile, mobilePattern);
            }
        }
    }
