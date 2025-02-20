using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services
{
    public class UserValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateUser(User user, string confirmPassword)
        {
            if (string.IsNullOrWhiteSpace(user.Email) || !IsValidEmail(user.Email))
                return (false, "Invalid email format.");

            if (string.IsNullOrWhiteSpace(user.Password) || user.Password.Length < 6)
                return (false, "Password must be at least 6 characters long.");

            if (user.Password != confirmPassword)
                return (false, "Passwords do not match.");

            if (string.IsNullOrWhiteSpace(user.FirstName))
                return (false, "First name is required.");

            if (string.IsNullOrWhiteSpace(user.LastName))
                return (false, "Last name is required.");

            if (user.RoleId <= 0)
                return (false, "Please select a valid role.");

            return (true, string.Empty);
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }
    }
}
