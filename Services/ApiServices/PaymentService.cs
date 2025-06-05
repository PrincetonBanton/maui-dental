using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class PaymentService : BaseApiService
    {
        public Task<bool> AddPaymentAsync(Payment payment)
            => PostAsync("Payment/AddPayment", payment);

        public Task<List<Payment>> GetPaymentsAsync(int patientId)
            => GetAsync<List<Payment>>($"Payment/GetPayments/{patientId}") ?? Task.FromResult(new List<Payment>());
    }
}
