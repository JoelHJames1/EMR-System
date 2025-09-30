using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IPatientService _patientService;
        private readonly ILogger<PatientController> _logger;

        public PatientController(IPatientService patientService, ILogger<PatientController> logger)
        {
            _patientService = patientService;
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
                var (patients, totalCount) = await _patientService.GetPatientsAsync(page, pageSize, search);
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
                var patient = await _patientService.GetPatientByIdAsync(id);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdPatient = await _patientService.CreatePatientAsync(patient, userId);

                _logger.LogInformation($"Patient created: {createdPatient.Id}");

                return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.Id }, createdPatient);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _patientService.UpdatePatientAsync(id, patient, userId);

                _logger.LogInformation($"Patient updated: {id}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Patient not found" });
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _patientService.DeletePatientAsync(id, userId);

                _logger.LogInformation($"Patient deactivated: {id}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Patient not found" });
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
                var allergies = await _patientService.GetPatientAllergiesAsync(id);
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
                var vitals = await _patientService.GetPatientVitalsAsync(id);
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