using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.DTOs
{
    public class EmployeeDTO
    {
        public int Id { get; set; }

        [Required]
        public required string FirstName { get; set; }

        public required string MiddleName { get; set; }

        [Required]
        public required string LastName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        public required string Gender { get; set; }

        [Required]
        public required string Address { get; set; }

        public required List<JobPositionDTO> JobPositions { get; set; }
    }
}