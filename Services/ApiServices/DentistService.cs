using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class DentistService : BaseApiService
    {
        public async Task<List<DentistVM>> GetDentistsAsync()
        {
            var dentists = await GetAsync<List<DentistVM>>("Dentist/GetAll") ?? new List<DentistVM>();
            return dentists;
        }

        public Task<bool> CreateDentistAsync(DentistVM dentist)
            => PostAsync("Dentist/Create", dentist);

        public Task<bool> UpdateDentistAsync(DentistVM dentist)
            => PutAsync($"Dentist/Update/{dentist.Id}", dentist);

        public Task<bool> DeleteDentistAsync(int id)
            => DeleteAsync($"Dentist/Delete/{id}");
    }
}
