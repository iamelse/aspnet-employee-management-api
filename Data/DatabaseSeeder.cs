using EmployeeManagementApi.Factories;
using EmployeeManagementApi.Services;

namespace EmployeeManagementApi.Data
{
    public class DatabaseSeeder
    {
        private readonly AppDbContext _context;
        private readonly EmployeeFactory _employeeFactory;

        public DatabaseSeeder(AppDbContext context, EncryptionService encryptionService)
        {
            _context = context;
            _employeeFactory = new EmployeeFactory(encryptionService);
        }

        public void Seed()
        {
            if (!_context.Employees.Any())
            {
                var employees = _employeeFactory.Generate(20); // generate 20 employees
                _context.Employees.AddRange(employees);
                _context.SaveChanges();
            }
        }
    }
}