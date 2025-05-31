using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Models
{
    public class Employee
    {
        public int Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        public required string MiddleName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        public required string Address { get; set; }

        [Required]
        public required byte[] EncryptedDateOfBirth { get; set; }

        public required List<JobPosition> JobPositions { get; set; }
    }
}