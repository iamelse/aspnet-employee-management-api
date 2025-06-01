using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.DTOs
{
    public class JobPositionDTO
    {
        public int Id { get; set; }

        [Required]
        public required string JobName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }
        
        public DateTime? EndDate { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "Salary must be positive")]
        public decimal Salary { get; set; }

        [Required]
        public required string Status { get; set; }
    }
}