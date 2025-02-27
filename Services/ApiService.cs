﻿using DentalApp.Models;
using System.Net.Http.Json;

namespace DentalApp.Services
{
    public class ApiService
    {
        private const string BaseUrl = "https://localhost:7078";
        private readonly HttpClient _httpClient = new();

        // Generic method to handle requests
        private async Task<T?> RequestAsync<T>(Func<Task<T?>> request, string errorMessage)
        {
            try
            {
                return await request();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{errorMessage}: {ex.Message}");
                return default;
            }
        }

        // GET request
        private Task<T?> GetAsync<T>(string endpoint)
            => RequestAsync(() => _httpClient.GetFromJsonAsync<T>($"{BaseUrl}/{endpoint}"), "Error fetching data");

        // POST request
        private Task<bool> PostAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PostAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error creating data");

        // PUT request
        private Task<bool> PutAsync<T>(string endpoint, T data)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.PutAsJsonAsync($"{BaseUrl}/{endpoint}", data);
                return response.IsSuccessStatusCode;
            }, "Error updating data");

        // DELETE request
        private Task<bool> DeleteAsync(string endpoint)
            => RequestAsync(async () =>
            {
                var response = await _httpClient.DeleteAsync($"{BaseUrl}/{endpoint}");
                return response.IsSuccessStatusCode;
            }, "Error deleting data");


        // User Methods
        public async Task<List<User>> GetUsersAsync()
        {
            var users = await GetAsync<List<User>>("User/GetAll") ?? new List<User>();
            return users.OrderBy(u => u.FullName).ToList();
        }
        public async Task<List<User>> GetDentistsAsync()
        {
            var users = await GetAsync<List<User>>("User/GetAll") ?? new List<User>();
            return users.Where(u => u.RoleName == "Dentist").OrderBy(u => u.FullName).ToList();
        }
        public async Task<List<User>> GetStaffAsync()
        {
            var users = await GetAsync<List<User>>("User/GetAll") ?? new List<User>();
            return users.Where(u => u.RoleName == "Staff").OrderBy(u => u.FullName).ToList();
        }

        public Task<User?> GetUserByIdAsync(int id) => GetAsync<User>($"User/Get/{id}");
        public Task<bool> CreateUserAsync(User user) => PostAsync("User/Create", user);
        public Task<bool> UpdateUserAsync(User user) => PutAsync($"User/Update/{user.Id}", user);
        public Task<bool> DeleteUserAsync(int id) => DeleteAsync($"User/Delete/{id}");
        public Task<bool> RegisterUserAsync(User user) => PostAsync("Account/Register", user);

        //Patients
        public async Task<List<PatientVM>> GetPatientsAsync()
        {
            var patients = await GetAsync<List<PatientVM>>("Patient/GetAll") ?? new List<PatientVM>();
            return patients;
        }
        public Task<bool> CreatePatientAsync(PatientVM patient) => PostAsync("Patient/Create", patient);
        public Task<bool> UpdatePatientAsync(PatientVM patient) => PutAsync($"Patient/Update/{patient.Id}", patient);
        public Task<bool> DeletePatientAsync(int id) => DeleteAsync($"Patient/Delete/{id}");



        //Role
        public Task<List<Role>> GetRolesAsync() 
            => GetAsync<List<Role>>("Role/GetAll") ?? Task.FromResult(new List<Role>());

        // Product
        public async Task<List<Product>> GetProductsAsync()
        {
            var products = await GetAsync<List<Product>>("Product/GetAll") ?? new List<Product>();
            return products.OrderBy(p => p.Name).ToList();
        }

        // Expense Category Methods
        public Task<List<ExpenseCategory>> GetExpenseCategoryAsync()
            => GetAsync<List<ExpenseCategory>>("Expense/GetCategories") ?? Task.FromResult(new List<ExpenseCategory>());

        // Expense Methods
        public async Task<List<Expense>> GetExpensesAsync()
        {
            var expenses = await GetAsync<List<Expense>>("Expense/GetAll") ?? new List<Expense>();
            return expenses.OrderByDescending(e => e.Id).ToList();
        }
        public Task<bool> CreateExpenseAsync(Expense expense) => PostAsync("Expense/Create", expense);
        public Task<bool> UpdateExpenseAsync(Expense expense) => PutAsync($"Expense/Update/{expense.Id}", expense);
        public Task<bool> DeleteExpenseAsync(int id)=> DeleteAsync($"Expense/Delete/{id}");
    }
}
