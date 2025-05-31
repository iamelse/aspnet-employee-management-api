using Bogus;
using EmployeeManagementApi.Models;

namespace EmployeeManagementApi.Factories
{
    public class JobPositionFactory
    {
        private Faker<JobPosition> faker;

        public JobPositionFactory()
        {
            faker = new Faker<JobPosition>()
                .RuleFor(j => j.Id, f => 0)
                .RuleFor(j => j.JobName, f => f.Name.JobTitle())
                .RuleFor(j => j.StartDate, f => f.Date.Past(5))
                .RuleFor(j => j.EndDate, (f, j) =>
                {
                    return f.Random.Bool(0.5f) ? (DateTime?)null : f.Date.Between(j.StartDate, DateTime.Now);
                })
                .RuleFor(j => j.Salary, f => Math.Round(f.Finance.Amount(3000, 10000), 2))
                .RuleFor(j => j.Status, (f, j) => j.EndDate == null ? "active" : "inactive")
                .RuleFor(j => j.EmployeeId, f => 0)
                .RuleFor(j => j.Employee, f => null!);
        }

        public JobPosition Generate(int employeeId)
        {
            var job = faker.Generate();
            job.EmployeeId = employeeId;
            return job;
        }
    }
}