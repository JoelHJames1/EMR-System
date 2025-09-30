using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PrescriptionController : ControllerBase
    {
        private readonly IPrescriptionService _prescriptionService;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(IPrescriptionService prescriptionService, ILogger<PrescriptionController> logger)
        {
            _prescriptionService = prescriptionService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient prescriptions
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPatientPrescriptions(
            int patientId,
            [FromQuery] string? status = null)
        {
            try
            {
                var prescriptions = await _prescriptionService.GetPatientPrescriptionsAsync(patientId, status);
                return Ok(prescriptions);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving prescriptions for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving prescriptions" });
            }
        }

        /// <summary>
        /// Create new prescription
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<Prescription>> CreatePrescription([FromBody] Prescription prescription)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdPrescription = await _prescriptionService.CreatePrescriptionAsync(prescription, userId);

                _logger.LogInformation($"Prescription created: {createdPrescription.Id}");

                return CreatedAtAction(nameof(GetPatientPrescriptions),
                    new { patientId = createdPrescription.PatientId }, createdPrescription);
            }
            catch (KeyNotFoundException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating prescription");
                return StatusCode(500, new { message = "Error creating prescription" });
            }
        }

        /// <summary>
        /// Update prescription status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdatePrescriptionStatus(
            int id,
            [FromBody] string status)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _prescriptionService.UpdatePrescriptionStatusAsync(id, status, userId);

                _logger.LogInformation($"Prescription {id} status updated to {status}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Prescription not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating prescription {id}");
                return StatusCode(500, new { message = "Error updating prescription" });
            }
        }

        /// <summary>
        /// Get all medications
        /// </summary>
        [HttpGet("medications")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Medication>>> GetMedications(
            [FromQuery] string? search = null)
        {
            try
            {
                var medications = await _prescriptionService.GetMedicationsAsync(search);
                return Ok(medications);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving medications");
                return StatusCode(500, new { message = "Error retrieving medications" });
            }
        }
    }
}