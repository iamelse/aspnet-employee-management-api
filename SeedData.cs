using EmployeeManagementApi.Data;
using EmployeeManagementApi.Factories;
using EmployeeManagementApi.Services;

namespace EmployeeManagementApi
{
    public static class SeedData
    {
        public static async Task Initialize(AppDbContext context, EncryptionService encryptionService)
        {
            if (context.Employees.Any())
            {
                // Data sudah ada, skip seeding
                return;
            }

            // Buat EmployeeFactory dengan encryptionService
            var employeeFactory = new EmployeeFactory(encryptionService);

            var employees = employeeFactory.Generate(50);

            await context.Employees.AddRangeAsync(employees);
            await context.SaveChangesAsync();
        }
    }
}