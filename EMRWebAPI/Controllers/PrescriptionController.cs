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
    public class PrescriptionController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<PrescriptionController> _logger;

        public PrescriptionController(EMRDBContext context, ILogger<PrescriptionController> logger)
        {
            _context = context;
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
                var query = _context.Prescriptions
                    .Include(p => p.Medication)
                    .Include(p => p.Provider)
                    .Where(p => p.PatientId == patientId);

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(p => p.Status == status);
                }

                var prescriptions = await query
                    .OrderByDescending(p => p.StartDate)
                    .ToListAsync();

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

                // Check if medication is controlled substance
                var medication = await _context.Medications.FindAsync(prescription.MedicationId);
                if (medication == null)
                {
                    return BadRequest(new { message = "Medication not found" });
                }

                if (medication.IsControlledSubstance)
                {
                    _logger.LogWarning($"Controlled substance prescribed: {medication.Name} (DEA: {medication.DEASchedule})");
                }

                prescription.CreatedDate = DateTime.UtcNow;
                prescription.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                prescription.StartDate = DateTime.UtcNow;

                _context.Prescriptions.Add(prescription);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Prescription created: {prescription.Id}");

                return CreatedAtAction(nameof(GetPatientPrescriptions),
                    new { patientId = prescription.PatientId }, prescription);
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
                var prescription = await _context.Prescriptions.FindAsync(id);
                if (prescription == null)
                {
                    return NotFound(new { message = "Prescription not found" });
                }

                prescription.Status = status;
                prescription.ModifiedDate = DateTime.UtcNow;
                prescription.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (status == "Discontinued" || status == "Completed")
                {
                    prescription.EndDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Prescription {id} status updated to {status}");

                return NoContent();
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
                var query = _context.Medications.AsQueryable();

                if (!string.IsNullOrEmpty(search))
                {
                    query = query.Where(m =>
                        m.Name.Contains(search) ||
                        m.GenericName.Contains(search) ||
                        (m.BrandName != null && m.BrandName.Contains(search)));
                }

                var medications = await query
                    .Where(m => m.IsActive)
                    .OrderBy(m => m.Name)
                    .Take(100)
                    .ToListAsync();

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