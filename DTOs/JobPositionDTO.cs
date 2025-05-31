namespace EmployeeManagementApi.DTOs
{
    public class JobPositionDTO
    {
        public int Id { get; set; }
        public required string JobName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal Salary { get; set; }
        public required string Status { get; set; }
    }
}