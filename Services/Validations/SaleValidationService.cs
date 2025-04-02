using System.Collections.Generic;
using DentalApp.Models;

namespace DentalApp.Services.Validations
{
    public static class SalesValidationService
    {
        public static (bool IsValid, string ErrorMessage) ValidateSale(int? patientId, int? dentistId, List<SaleLine> selectedProducts)
        {
            if (patientId == null || patientId == 0)
                return (false, "Please select a patient.");

            if (dentistId == null || dentistId == 0)
                return (false, "Please select a dentist.");

            if (selectedProducts == null || selectedProducts.Count == 0)
                return (false, "Please add at least one product or service before saving.");

            return (true, string.Empty);
        }
    }
}
