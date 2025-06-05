using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class PatientService : BaseApiService
    {
        public async Task<List<PatientVM>> GetPatientsAsync()
        {
            var patients = await GetAsync<List<PatientVM>>("Patient/GetAll") ?? new List<PatientVM>();
            return patients;
        }

        public Task<bool> CreatePatientAsync(PatientVM patient)
            => PostAsync("Patient/Create", patient);

        public Task<bool> UpdatePatientAsync(PatientVM patient)
            => PutAsync($"Patient/Update/{patient.Id}", patient);

        public Task<bool> DeletePatientAsync(int id)
            => DeleteAsync($"Patient/Delete/{id}");
    }
}
