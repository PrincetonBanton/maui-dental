using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class UserService : BaseApiService
    {
        public Task<List<Role>> GetRolesAsync()
            => GetAsync<List<Role>>("Role/GetAll") ?? Task.FromResult(new List<Role>());

        public Task<List<UserVM>> GetUsersAsync()
            => GetAsync<List<UserVM>>("User/GetAll") ?? Task.FromResult(new List<UserVM>());

        public Task<UserVM?> GetUserByIdAsync(int userId)
            => GetAsync<UserVM>($"User/Get/{userId}");

        public Task<bool> CreateUserAsync(UserVM user)
            => PostAsync("Account/Register", user);

        public Task<bool> UpdateUserAsync(UserVM user)
            => PutAsync($"User/Update/{user.Id}", user);

        public Task<bool> DeleteUserAsync(int id)
            => DeleteAsync($"User/Delete/{id}");
    }
}
