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
        /// Mengambil seluruh data karyawan beserta posisinya.
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<EmployeeDTO>>> GetEmployees()
        {
            try
            {
                var employees = await _context.Employees
                    .Include(e => e.JobPositions)
                    .ToListAsync();

                var employeeDTOs = employees.Select(e => new EmployeeDTO
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
                });

                return Ok(employeeDTOs);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Terjadi kesalahan saat mengambil data: {ex.Message}");
            }
        }

        /// <summary>
        /// Menambahkan karyawan baru ke dalam database.
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<EmployeeDTO>> CreateEmployee(EmployeeDTO employeeDTO)
        {
            try
            {
                if (employeeDTO.JobPositions != null &&
                    employeeDTO.JobPositions.Count(j => j.Status == "active") > 1)
                {
                    return BadRequest("Only one active job position is allowed per employee.");
                }

                var employee = new Employee
                {
                    FirstName = employeeDTO.FirstName,
                    MiddleName = employeeDTO.MiddleName,
                    LastName = employeeDTO.LastName,
                    EncryptedDateOfBirth = _encryptionService.Encrypt(employeeDTO.DateOfBirth.ToString("yyyy-MM-dd")),
                    Gender = employeeDTO.Gender,
                    Address = employeeDTO.Address,
                    JobPositions = new List<JobPosition>()
                };

                // Tambahkan posisi kerja sekaligus set Employee (back-reference)
                if (employeeDTO.JobPositions != null)
                {
                    employee.JobPositions = employeeDTO.JobPositions.Select(j => new JobPosition
                    {
                        JobName = j.JobName,
                        StartDate = j.StartDate,
                        EndDate = j.EndDate,
                        Salary = j.Salary,
                        Status = j.Status,
                        Employee = employee // ✅ wajib karena properti required
                    }).ToList();
                }

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

                if (employee == null)
                {
                    return NotFound();
                }

                if (employeeDTO.JobPositions != null &&
                    employeeDTO.JobPositions.Count(j => j.Status == "active") > 1)
                {
                    return BadRequest("Only one active job position is allowed per employee.");
                }

                employee.FirstName = employeeDTO.FirstName;
                employee.MiddleName = employeeDTO.MiddleName;
                employee.LastName = employeeDTO.LastName;
                employee.Gender = employeeDTO.Gender;
                employee.Address = employeeDTO.Address;
                employee.EncryptedDateOfBirth = _encryptionService.Encrypt(employeeDTO.DateOfBirth.ToString("yyyy-MM-dd"));

                // Hapus dan tambahkan kembali semua JobPosition dengan referensi yang benar
                employee.JobPositions.Clear();
                if (employeeDTO.JobPositions != null)
                {
                    employee.JobPositions = employeeDTO.JobPositions.Select(j => new JobPosition
                    {
                        JobName = j.JobName,
                        StartDate = j.StartDate,
                        EndDate = j.EndDate,
                        Salary = j.Salary,
                        Status = j.Status,
                        Employee = employee // ✅ back-reference wajib
                    }).ToList();
                }

                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal memperbarui data karyawan: {ex.Message}");
            }
        }

        /// <summary>
        /// Menghapus data karyawan berdasarkan ID.
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteEmployee(int id)
        {
            try
            {
                var employee = await _context.Employees
                    .Include(e => e.JobPositions)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (employee == null)
                {
                    return NotFound();
                }

                _context.Employees.Remove(employee);
                await _context.SaveChangesAsync();
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Gagal menghapus data karyawan: {ex.Message}");
            }
        }
    }
}