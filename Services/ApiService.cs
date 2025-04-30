using DentalApp.Models;
using System.Net.Http.Json;
using System.Text.Json;
using System.Text;

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
                await Shell.Current.DisplayAlert("Error", $"{errorMessage}: {ex.Message}", "OK");
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


        // User
        public Task<List<Role>> GetRolesAsync()
            => GetAsync<List<Role>>("Role/GetAll") ?? Task.FromResult(new List<Role>());
        public async Task<List<UserVM>> GetUsersAsync()
        {
            var users = await GetAsync<List<UserVM>>("User/GetAll") ?? new List<UserVM>();
            return users;
        }
        public Task<bool> CreateUserAsync(UserVM user) => PostAsync("Account/Register", user);
        public Task<UserVM?> GetUserByIdAsync(int userId) => GetAsync<UserVM>($"User/Get/{userId}");
        public Task<bool> UpdateUserAsync(UserVM user) => PutAsync($"User/Update/{user.Id}", user);
        public Task<bool> DeleteUserAsync(int id) => DeleteAsync($"User/Delete/{id}");


        //Patients
        public async Task<List<PatientVM>> GetPatientsAsync()
        {
            var patients = await GetAsync<List<PatientVM>>("Patient/GetAll") ?? new List<PatientVM>();
            return patients;
        }
        public Task<bool> CreatePatientAsync(PatientVM patient) => PostAsync("Patient/Create", patient);
        public Task<bool> UpdatePatientAsync(PatientVM patient) => PutAsync($"Patient/Update/{patient.Id}", patient);
        public Task<bool> DeletePatientAsync(int id) => DeleteAsync($"Patient/Delete/{id}");

        //Dentist
        public async Task<List<DentistVM>> GetDentistsAsync()
        {
            var dentists = await GetAsync<List<DentistVM>>("Dentist/GetAll") ?? new List<DentistVM>();
            return dentists;
        }
        public Task<bool> CreateDentistAsync(DentistVM dentist) => PostAsync("Dentist/Create", dentist);
        public Task<bool> UpdateDentistAsync(DentistVM dentist) => PutAsync($"Dentist/Update/{dentist.Id}", dentist);
        public Task<bool> DeleteDentistAsync(int id) => DeleteAsync($"Dentist/Delete/{id}");

        // Product
        public async Task<List<ProductVM>> GetProductsAsync()
        {
            var products = await GetAsync<List<ProductVM>>("Product/GetAll") ?? new List<ProductVM>();
            return products.OrderBy(p => p.Name).ToList();
        }
        public Task<bool> CreateProductAsync(ProductVM product) => PostAsync("Product/Create", product);
        public Task<bool> UpdateProductAsync(ProductVM product) => PutAsync($"Product/Update/{product.Id}", product);
        public Task<bool> DeleteProductAsync(int id) => DeleteAsync($"Product/Delete/{id}");

        // Expense 
        public Task<List<ExpenseCategory>> GetExpenseCategoryAsync()
            => GetAsync<List<ExpenseCategory>>("Expense/GetCategories") ?? Task.FromResult(new List<ExpenseCategory>());
        public async Task<List<Expense>> GetExpensesAsync()
        {
            var expenses = await GetAsync<List<Expense>>("Expense/GetAll") ?? new List<Expense>();
            return expenses.OrderByDescending(e => e.Id).ToList();
        }
        public Task<bool> CreateExpenseAsync(Expense expense) => PostAsync("Expense/Create", expense);
        public Task<bool> UpdateExpenseAsync(Expense expense) => PutAsync($"Expense/Update/{expense.Id}", expense);
        public Task<bool> DeleteExpenseAsync(int id) => DeleteAsync($"Expense/Delete/{id}");

        //Sale
        public async Task<List<SaleVM>> GetSalesAsync()
        {
            try
            {
                var sales = await GetAsync<List<SaleVM>>("Sale/GetAll");

                if (sales == null)
                {
                    await Shell.Current.DisplayAlert("Error", "Unable to load sales. The server returned no data.", "OK");
                    return new List<SaleVM>();
                }

                return sales.OrderByDescending(e => e.SaleId).ToList();
            }
            catch (Exception ex)
            {
                await Shell.Current.DisplayAlert("Error", $"An error occurred while fetching sales:\n{ex.Message}", "OK");
                return new List<SaleVM>();
            }
        }

        public Task<SaleVM?> GetSaleDetailAsync(int id) => GetAsync<SaleVM>($"Sale/GetDetail/{id}");
        public Task<bool> CreateSaleAsync(SaleVM sale) => PostAsync("Sale/Create", sale);
        public Task<bool> DeleteSaleAsync(int id) => DeleteAsync($"Sale/Delete/{id}");

        //Payment
        public Task<bool> AddPaymentAsync(Payment payment) => PostAsync("Payment/AddPayment", payment);

        //Supplier
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            var suppliers = await GetAsync<List<Supplier>>("Supplier/GetAll") ?? new List<Supplier>();
            return suppliers.OrderByDescending(e => e.Id).ToList();
        }
        public Task<bool> CreateSupplierAsync(Supplier supplier) => PostAsync("Supplier/Create", supplier);
        public Task<bool> UpdateSupplierAsync(Supplier supplier) => PutAsync($"Supplier/Update/{supplier.Id}", supplier);
        public Task<bool> DeleteSupplierAsync(int id) => DeleteAsync($"Supplier/Delete/{id}");

        //Appointment
        public async Task<List<Appointment>> GetAppointmentsAsync()
        {
            var appointments = await GetAsync<List<Appointment>>("Appointment/GetAll") ?? new List<Appointment>();
            return appointments.OrderByDescending(e => e.Id).ToList();
        }
        public Task<bool> CreateAppointmentAsync(Appointment appointment) => PostAsync("Appointment/Create", appointment);
        public Task<bool> UpdateAppointmentAsync(Appointment appointment) => PutAsync($"Appointment/Update/{appointment.Id}", appointment);
        public Task<bool> DeleteAppointmentAsync(int id) => DeleteAsync($"Appointment/Delete/{id}");


        //Auth
        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var loginData = new { Username = username, Password = password };
            var json = JsonSerializer.Serialize(loginData);
            var content = new StringContent(json, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("your-api-endpoint/login", content);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                // assuming your API returns a token in plain string or JSON { token: "..." }
                return result; // parse appropriately
            }

            return null;
        }


    }
}
