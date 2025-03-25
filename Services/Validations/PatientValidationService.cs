using DentalApp.Models;
using System.Text.RegularExpressions;

namespace DentalApp.Services.Validations
{
    public static class PatientValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidatePatient(PatientVM patient)
        {
            if (string.IsNullOrWhiteSpace(patient.FirstName))
                return (false, "First name is required.");
            if (string.IsNullOrWhiteSpace(patient.MiddleName))
                //return (false, "Middle Name is required.");
                patient.MiddleName = "-";
            if (string.IsNullOrWhiteSpace(patient.LastName))
                return (false, "Last name is required.");

            if (string.IsNullOrWhiteSpace(patient.Address))
                return (false, "Address is required.");

            if (patient.BirthDate == default)
                return (false, "A valid birth date is required.");

            if (!IsValidEmail(patient.Email))
                return (false, "Invalid email format.");

            if (!IsValidMobileNumber(patient.Mobile))
                return (false, "Invalid mobile number format.");

            if (string.IsNullOrWhiteSpace(patient.PatientNo))
                //return (false, "Patient number is required.");
                patient.PatientNo = "-";

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
