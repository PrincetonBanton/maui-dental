using DentalApp.Models;
using System.Collections.ObjectModel;

namespace DentalApp.Data
{
    public static class CreateSale
    {
        public static SaleVM BuildSale(PatientVM patient, DentistVM dentist, ObservableCollection<SaleLine> items, decimal payment, DateTime saleDate)
        {
            var total = items.Sum(i => i.SubTotal);

            return new SaleVM
            {
                SaleNo = $"SALE-{DateTime.UtcNow:yyyyMMdd-HHmmss}",
                SaleDate = saleDate,
                PatientId = patient?.Id ?? 0,
                DentistId = dentist?.Id ?? 0,
                Note = "Patient purchased treatments",  
                SubTotal = total,
                Total = total,
                Items = items.Select(i => new SaleVM.SaleItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    Amount = i.SubTotal
                }).ToList(),
                Payments = new()
                {
                    new SaleVM.SalePayment
                    {
                        PaymentAmount = payment,
                        PaymentType = 0,
                        AmountTendered = payment,
                        EnteredBy = 5,
                        PaymentDate = DateTime.UtcNow
                    }
                }
            };
        }
    }
}
