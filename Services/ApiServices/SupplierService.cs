﻿using DentalApp.Models;

namespace DentalApp.Services.ApiServices
{
    public class SupplierService : BaseApiService
    {
        public async Task<List<Supplier>> GetSuppliersAsync()
        {
            var suppliers = await GetAsync<List<Supplier>>("Supplier/GetAll") ?? new List<Supplier>();
            return suppliers.OrderByDescending(s => s.Id).ToList();
        }

        public Task<bool> CreateSupplierAsync(Supplier supplier)
            => PostAsync("Supplier/Create", supplier);

        public Task<bool> UpdateSupplierAsync(Supplier supplier)
            => PutAsync($"Supplier/Update/{supplier.Id}", supplier);

        public Task<bool> DeleteSupplierAsync(int id)
            => DeleteAsync($"Supplier/Delete/{id}");
    }
}
