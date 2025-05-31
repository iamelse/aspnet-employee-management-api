using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Models
{
    public class Employee
    {
        public int Id { get; set; }

        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Address { get; set; } = null!;
        public byte[] EncryptedDateOfBirth { get; set; } = null!;
        public List<JobPosition> JobPositions { get; set; } = new List<JobPosition>();

        // Constructor dengan 7 parameter
        public Employee(string firstName, string middleName, string lastName, string gender, string address, byte[] encryptedDateOfBirth, List<JobPosition> jobPositions)
        {
            FirstName = firstName;
            MiddleName = middleName;
            LastName = lastName;
            Gender = gender;
            Address = address;
            EncryptedDateOfBirth = encryptedDateOfBirth;
            JobPositions = jobPositions;
        }

        // Constructor parameterless supaya EF Core tetap bisa pakai
        public Employee()
        {
            
        }
    }
}