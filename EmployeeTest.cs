using EmployeeManagementApi.Models;

namespace EmployeeManagementApi.Tests
{
    public class EmployeeTests
    {
        private Employee CreateEmployeeWithJobs(List<JobPosition> jobs)
        {
            return new Employee
            {
                FirstName = "Test",
                MiddleName = "M",
                LastName = "User",
                Gender = "Other",
                Address = "Test Address",
                EncryptedDateOfBirth = System.Text.Encoding.UTF8.GetBytes("EncryptedDOB123"),
                JobPositions = jobs
            };
        }

        // Fungsi validasi yang bisa digunakan di test dan juga di service sebenarnya
        private bool ValidateActiveJobPositions(Employee employee, out string errorMessage)
        {
            int activeCount = employee.JobPositions.Count(j => j.Status != null && j.Status.ToLower() == "active");
            if (activeCount > 1)
            {
                errorMessage = "More than one active job position is not allowed.";
                return false;
            }
            errorMessage = null;
            return true;
        }

        [Fact]
        public void Employee_ShouldHaveOnlyOneActiveJob()
        {
            var employee = CreateEmployeeWithJobs(new List<JobPosition>
            {
                new JobPosition { JobName = "Developer", Status = "active", Employee = null },
                new JobPosition { JobName = "Lead", Status = "inactive", Employee = null }
            });

            var isValid = ValidateActiveJobPositions(employee, out var errorMessage);

            Assert.True(isValid, errorMessage);
        }

        [Fact]
        public void Employee_WithMultipleActiveJobs_ShouldFailValidation()
        {
            var employee = CreateEmployeeWithJobs(new List<JobPosition>
            {
                new JobPosition { JobName = "Dev", Status = "active", Employee = null },
                new JobPosition { JobName = "Lead", Status = "active", Employee = null }
            });

            var isValid = ValidateActiveJobPositions(employee, out var errorMessage);

            Assert.False(isValid);
            Assert.Equal("More than one active job position is not allowed.", errorMessage);
        }
    }
}