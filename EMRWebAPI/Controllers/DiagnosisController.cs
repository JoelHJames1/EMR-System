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
    public class DiagnosisController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<DiagnosisController> _logger;

        public DiagnosisController(EMRDBContext context, ILogger<DiagnosisController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient diagnoses
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Diagnosis>>> GetPatientDiagnoses(int patientId)
        {
            try
            {
                var diagnoses = await _context.Diagnoses
                    .Include(d => d.Provider)
                    .Where(d => d.PatientId == patientId)
                    .OrderByDescending(d => d.DiagnosisDate)
                    .ToListAsync();

                return Ok(diagnoses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving diagnoses for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving diagnoses" });
            }
        }

        /// <summary>
        /// Create new diagnosis
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<Diagnosis>> CreateDiagnosis([FromBody] Diagnosis diagnosis)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                diagnosis.CreatedDate = DateTime.UtcNow;
                diagnosis.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
                diagnosis.DiagnosisDate = DateTime.UtcNow;

                _context.Diagnoses.Add(diagnosis);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Diagnosis created: {diagnosis.Id}");

                return CreatedAtAction(nameof(GetPatientDiagnoses),
                    new { patientId = diagnosis.PatientId }, diagnosis);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating diagnosis");
                return StatusCode(500, new { message = "Error creating diagnosis" });
            }
        }

        /// <summary>
        /// Update diagnosis
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> UpdateDiagnosis(int id, [FromBody] Diagnosis diagnosis)
        {
            try
            {
                if (id != diagnosis.Id)
                {
                    return BadRequest(new { message = "Diagnosis ID mismatch" });
                }

                var existingDiagnosis = await _context.Diagnoses.FindAsync(id);
                if (existingDiagnosis == null)
                {
                    return NotFound(new { message = "Diagnosis not found" });
                }

                existingDiagnosis.ClinicalStatus = diagnosis.ClinicalStatus;
                existingDiagnosis.VerificationStatus = diagnosis.VerificationStatus;
                existingDiagnosis.Severity = diagnosis.Severity;
                existingDiagnosis.Notes = diagnosis.Notes;
                existingDiagnosis.ModifiedDate = DateTime.UtcNow;
                existingDiagnosis.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating diagnosis {id}");
                return StatusCode(500, new { message = "Error updating diagnosis" });
            }
        }
    }
}