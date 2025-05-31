using System.ComponentModel.DataAnnotations;

namespace EmployeeManagementApi.Models
{
    public class JobPosition
    {
        public int Id { get; set; }

        [Required]
        public required string JobName { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required]
        public decimal Salary { get; set; }

        [Required]
        public required string Status { get; set; }  // "active" or "inactive"

        public int EmployeeId { get; set; }
        public required Employee Employee { get; set; }
    }
}