using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ImmunizationController : ControllerBase
    {
        private readonly IImmunizationService _immunizationService;
        private readonly ILogger<ImmunizationController> _logger;

        public ImmunizationController(IImmunizationService immunizationService, ILogger<ImmunizationController> logger)
        {
            _immunizationService = immunizationService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient immunizations
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Immunization>>> GetPatientImmunizations(int patientId)
        {
            try
            {
                var immunizations = await _immunizationService.GetPatientImmunizationsAsync(patientId);
                return Ok(immunizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunizations for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving immunizations" });
            }
        }

        /// <summary>
        /// Record immunization
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Immunization>> RecordImmunization([FromBody] Immunization immunization)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdImmunization = await _immunizationService.RecordImmunizationAsync(immunization, userId);

                _logger.LogInformation($"Immunization recorded for patient {createdImmunization.PatientId}: {createdImmunization.VaccineName}");

                return Ok(createdImmunization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording immunization");
                return StatusCode(500, new { message = "Error recording immunization" });
            }
        }

        /// <summary>
        /// Update immunization
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateImmunization(int id, [FromBody] Immunization immunization)
        {
            try
            {
                if (id != immunization.Id)
                {
                    return BadRequest(new { message = "Immunization ID mismatch" });
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _immunizationService.UpdateImmunizationAsync(id, immunization, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Immunization not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating immunization {id}");
                return StatusCode(500, new { message = "Error updating immunization" });
            }
        }

        /// <summary>
        /// Get immunization history report
        /// </summary>
        [HttpGet("patient/{patientId}/history")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<object>> GetImmunizationHistory(int patientId)
        {
            try
            {
                var history = await _immunizationService.GetImmunizationHistoryAsync(patientId);
                return Ok(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunization history for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving immunization history" });
            }
        }
    }
}