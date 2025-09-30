using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<PatientController> _logger;

        public PatientController(EMRDBContext context, ILogger<PatientController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all patients
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients(
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50,
            [FromQuery] string? search = null)
        {
            try
            {
                var query = _context.Patients
                    .Include(p => p.Address)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(p =>
                        p.FirstName.Contains(search) ||
                        p.LastName.Contains(search) ||
                        p.Email.Contains(search));
                }

                var totalCount = await query.CountAsync();
                var patients = await query
                    .Where(p => p.IsActive)
                    .OrderBy(p => p.LastName)
                    .Skip((page - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();

                Response.Headers.Add("X-Total-Count", totalCount.ToString());

                return Ok(patients);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving patients");
                return StatusCode(500, new { message = "Error retrieving patients" });
            }
        }

        /// <summary>
        /// Get patient by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            try
            {
                var patient = await _context.Patients
                    .Include(p => p.Address)
                    .Include(p => p.Allergies)
                    .Include(p => p.Immunizations)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                return Ok(patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving patient {id}");
                return StatusCode(500, new { message = "Error retrieving patient" });
            }
        }

        /// <summary>
        /// Register new patient
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Patient>> CreatePatient([FromBody] Patient patient)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                patient.CreatedDate = DateTime.UtcNow;
                patient.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Patients.Add(patient);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Patient created: {patient.Id}");

                return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating patient");
                return StatusCode(500, new { message = "Error creating patient" });
            }
        }

        /// <summary>
        /// Update patient information
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] Patient patient)
        {
            try
            {
                if (id != patient.Id)
                {
                    return BadRequest(new { message = "Patient ID mismatch" });
                }

                var existingPatient = await _context.Patients.FindAsync(id);
                if (existingPatient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                existingPatient.FirstName = patient.FirstName;
                existingPatient.LastName = patient.LastName;
                existingPatient.MiddleName = patient.MiddleName;
                existingPatient.DateOfBirth = patient.DateOfBirth;
                existingPatient.Gender = patient.Gender;
                existingPatient.Email = patient.Email;
                existingPatient.PhoneNumber = patient.PhoneNumber;
                existingPatient.EmergencyContact = patient.EmergencyContact;
                existingPatient.EmergencyContactName = patient.EmergencyContactName;
                existingPatient.BloodType = patient.BloodType;
                existingPatient.MaritalStatus = patient.MaritalStatus;
                existingPatient.ModifiedDate = DateTime.UtcNow;
                existingPatient.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Patient updated: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating patient {id}");
                return StatusCode(500, new { message = "Error updating patient" });
            }
        }

        /// <summary>
        /// Deactivate patient
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var patient = await _context.Patients.FindAsync(id);
                if (patient == null)
                {
                    return NotFound(new { message = "Patient not found" });
                }

                patient.IsActive = false;
                patient.ModifiedDate = DateTime.UtcNow;
                patient.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Patient deactivated: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting patient {id}");
                return StatusCode(500, new { message = "Error deleting patient" });
            }
        }

        /// <summary>
        /// Get patient's allergies
        /// </summary>
        [HttpGet("{id}/allergies")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Allergy>>> GetPatientAllergies(int id)
        {
            try
            {
                var allergies = await _context.Allergies
                    .Where(a => a.PatientId == id && a.IsActive)
                    .ToListAsync();

                return Ok(allergies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergies for patient {id}");
                return StatusCode(500, new { message = "Error retrieving allergies" });
            }
        }

        /// <summary>
        /// Get patient's vital signs
        /// </summary>
        [HttpGet("{id}/vitals")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<VitalSign>>> GetPatientVitals(int id)
        {
            try
            {
                var vitals = await _context.VitalSigns
                    .Where(v => v.PatientId == id)
                    .OrderByDescending(v => v.MeasurementDate)
                    .Take(10)
                    .ToListAsync();

                return Ok(vitals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vitals for patient {id}");
                return StatusCode(500, new { message = "Error retrieving vitals" });
            }
        }
    }
}