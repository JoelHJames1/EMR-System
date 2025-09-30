using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DiagnosisController : ControllerBase
    {
        private readonly IDiagnosisService _diagnosisService;
        private readonly ILogger<DiagnosisController> _logger;

        public DiagnosisController(IDiagnosisService diagnosisService, ILogger<DiagnosisController> logger)
        {
            _diagnosisService = diagnosisService;
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
                var diagnoses = await _diagnosisService.GetPatientDiagnosesAsync(patientId);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdDiagnosis = await _diagnosisService.CreateDiagnosisAsync(diagnosis, userId);

                _logger.LogInformation($"Diagnosis created: {createdDiagnosis.Id}");

                return CreatedAtAction(nameof(GetPatientDiagnoses),
                    new { patientId = createdDiagnosis.PatientId }, createdDiagnosis);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _diagnosisService.UpdateDiagnosisAsync(id, diagnosis, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Diagnosis not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating diagnosis {id}");
                return StatusCode(500, new { message = "Error updating diagnosis" });
            }
        }
    }
}