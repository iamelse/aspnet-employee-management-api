using Bogus;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Services;

namespace EmployeeManagementApi.Factories
{
    public class EmployeeFactory
    {
        private readonly EncryptionService _encryptionService;
        private readonly JobPositionFactory jobPositionFactory;
        private readonly Faker<Employee> faker;

        public EmployeeFactory(EncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            jobPositionFactory = new JobPositionFactory();

            faker = new Faker<Employee>()
                .CustomInstantiator(f =>
                {
                    var firstName = f.Name.FirstName();
                    var middleName = f.Name.FirstName();
                    var lastName = f.Name.LastName();
                    var gender = f.PickRandom(new[] { "Male", "Female", "Other" });
                    var address = f.Address.FullAddress();

                    var dob = f.Date.Past(50, DateTime.Now.AddYears(-18));
                    var encryptedDob = _encryptionService.Encrypt(dob.ToString("yyyy-MM-dd"));

                    var jobCount = f.Random.Int(1, 3);
                    var jobs = new List<JobPosition>();
                    var employee = new Employee(); // for reference in JobPositions

                    for (int i = 0; i < jobCount; i++)
                    {
                        var job = jobPositionFactory.Generate(employee.Id);
                        jobs.Add(job);
                    }

                    var emp = new Employee(
                        firstName,
                        middleName,
                        lastName,
                        gender,
                        address,
                        encryptedDob,
                        jobs
                    );

                    emp.Id = 0; // biar auto-increment di DB

                    foreach (var job in jobs)
                    {
                        job.Employee = emp;
                        job.EmployeeId = emp.Id;
                    }

                    return emp;
                });
        }

        public Employee Generate()
        {
            var employee = faker.Generate();

            foreach (var job in employee.JobPositions)
            {
                job.Employee = employee;
                job.EmployeeId = employee.Id;
            }

            return employee;
        }

        public List<Employee> Generate(int count)
        {
            var employees = new List<Employee>();
            for (int i = 0; i < count; i++)
            {
                var emp = Generate();
                employees.Add(emp);
            }
            return employees;
        }
    }
}