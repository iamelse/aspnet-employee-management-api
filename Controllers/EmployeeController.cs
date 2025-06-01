using EmployeeManagementApi.Data;
using EmployeeManagementApi.DTOs;
using EmployeeManagementApi.Models;
using EmployeeManagementApi.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EmployeeManagementApi.Controllers
{
    [ApiController]
    [Route("api/employees")]
    public class EmployeesController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly EncryptionService _encryptionService;

        public EmployeesController(AppDbContext context, EncryptionService encryptionService)
        {
            _context = context;
            _encryptionService = encryptionService;
        }

        /// <summary>
        /// Mendapatkan daftar karyawan dengan paging dan pencarian.
        /// Default: page 1, size 10.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult> GetEmployees(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchTerm = null)
        {
            try
            {
                (int validPage, int validSize) = ValidatePagingParameters(pageNumber, pageSize);

                var query = BuildEmployeeQuery(searchTerm);

                int totalItems = await query.CountAsync();

                var employees = await query
                    .OrderBy(e => e.Id)
                    .Skip((validPage - 1) * validSize)
                    .Take(validSize)
                    .ToListAsync();

                var employeeDTOs = employees.Select(MapEmployeeToDTO);

                var response = new
                {
                    TotalItems = totalItems,
                    PageNumber = validPage,
                    PageSize = validSize,
                    Items = employeeDTOs
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data: {ex.Message}");
            }
        }

        /// <summary>
        /// Menambahkan karyawan baru ke database.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployee([FromBody] EmployeeDTO employeeDTO)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!IsValidJobPositions(employeeDTO.JobPositions, out string? validationError))
                    return BadRequest(validationError);

                var employee = MapDTOToEmployee(employeeDTO);

                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                employeeDTO.Id = employee.Id;

                return CreatedAtAction(nameof(GetEmployees), new { id = employee.Id }, employeeDTO);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal menambahkan karyawan: {ex.Message}");
            }
        }

        /// <summary>
        /// Memperbarui data karyawan berdasarkan ID.
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateEmployee(int id, EmployeeDTO employeeDTO)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.JobPositions)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null) return NotFound();

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                if (!IsValidJobPositions(employeeDTO.JobPositions, out string? validationError))
                    return BadRequest(validationError);

                UpdateEmployeeFromDTO(employee, employeeDTO);

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal memperbarui data karyawan: {ex.Message}");
            }
        }

        /// <summary>
        /// Menghapus karyawan berdasarkan ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.JobPositions)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null) return NotFound();

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal menghapus data karyawan: {ex.Message}");
            }
        }

        #region Helper Methods

        private (int page, int size) ValidatePagingParameters(int pageNumber, int pageSize)
        {
            int page = pageNumber <= 0 ? 1 : pageNumber;
            int size = pageSize <= 0 ? 10 : pageSize;
            // Optional: Batasi max page size jika perlu, misal max 100
            size = size > 100 ? 100 : size;
            return (page, size);
        }

        private IQueryable<Employee> BuildEmployeeQuery(string? searchTerm)
        {
            var query = _context.Employees.Include(e => e.JobPositions).AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                string lowerTerm = searchTerm.ToLower();
                query = query.Where(e =>
                    e.FirstName.ToLower().Contains(lowerTerm) ||
                    e.MiddleName.ToLower().Contains(lowerTerm) ||
                    e.LastName.ToLower().Contains(lowerTerm));
            }

            return query;
        }

        private EmployeeDTO MapEmployeeToDTO(Employee e)
        {
            return new EmployeeDTO
            {
                Id = e.Id,
                FirstName = e.FirstName,
                MiddleName = e.MiddleName,
                LastName = e.LastName,
                DateOfBirth = DateTime.Parse(_encryptionService.Decrypt(e.EncryptedDateOfBirth)),
                Gender = e.Gender,
                Address = e.Address,
                JobPositions = e.JobPositions.Select(j => new JobPositionDTO
                {
                    Id = j.Id,
                    JobName = j.JobName,
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    Salary = j.Salary,
                    Status = j.Status
                }).ToList()
            };
        }

        private Employee MapDTOToEmployee(EmployeeDTO dto)
        {
            var employee = new Employee
            {
                FirstName = dto.FirstName,
                MiddleName = dto.MiddleName,
                LastName = dto.LastName,
                EncryptedDateOfBirth = _encryptionService.Encrypt(dto.DateOfBirth.ToString("yyyy-MM-dd")),
                Gender = dto.Gender,
                Address = dto.Address,
                JobPositions = new List<JobPosition>()
            };

            if (dto.JobPositions != null)
            {
                employee.JobPositions = dto.JobPositions.Select(j => new JobPosition
                {
                    JobName = j.JobName,
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    Salary = j.Salary,
                    Status = j.Status,
                    Employee = employee // back-reference wajib
                }).ToList();
            }

            return employee;
        }

        private void UpdateEmployeeFromDTO(Employee employee, EmployeeDTO dto)
        {
            employee.FirstName = dto.FirstName;
            employee.MiddleName = dto.MiddleName;
            employee.LastName = dto.LastName;
            employee.Gender = dto.Gender;
            employee.Address = dto.Address;
            employee.EncryptedDateOfBirth = _encryptionService.Encrypt(dto.DateOfBirth.ToString("yyyy-MM-dd"));

            // Clear existing job positions and add updated list
            employee.JobPositions.Clear();

            if (dto.JobPositions != null)
            {
                employee.JobPositions = dto.JobPositions.Select(j => new JobPosition
                {
                    JobName = j.JobName,
                    StartDate = j.StartDate,
                    EndDate = j.EndDate,
                    Salary = j.Salary,
                    Status = j.Status,
                    Employee = employee
                }).ToList();
            }
        }

        private bool IsValidJobPositions(List<JobPositionDTO>? jobPositions, out string? errorMessage)
        {
            errorMessage = null;
            if (jobPositions == null) return true;

            int activeCount = jobPositions.Count(j => j.Status.Equals("Active", StringComparison.OrdinalIgnoreCase));
            if (activeCount > 1)
            {
                errorMessage = "Only one active job position is allowed per employee.";
                return false;
            }

            return true;
        }

        #endregion
    }
}