using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class AppointmentService : BaseApiService
    {
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            var appointments = await GetAsync<List<Appointment>>("Appointment/GetAll") ?? new List<Appointment>();
            return appointments.OrderByDescending(e => e.Id).ToList();
        }

        public Task<bool> CreateAppointmentAsync(Appointment appointment)
            => PostAsync("Appointment/Create", appointment);

        public Task<bool> UpdateAppointmentAsync(Appointment appointment)
            => PutAsync($"Appointment/Update/{appointment.Id}", appointment);

        public Task<bool> DeleteAppointmentAsync(int id)
            => DeleteAsync($"Appointment/Delete/{id}");
    }
}
