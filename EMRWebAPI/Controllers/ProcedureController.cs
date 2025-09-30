using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProcedureController : ControllerBase
    {
        private readonly IProcedureService _procedureService;
        private readonly ILogger<ProcedureController> _logger;

        public ProcedureController(IProcedureService procedureService, ILogger<ProcedureController> logger)
        {
            _procedureService = procedureService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient procedures
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Procedure>>> GetPatientProcedures(int patientId)
        {
            try
            {
                var procedures = await _procedureService.GetPatientProceduresAsync(patientId);
                return Ok(procedures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving procedures for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving procedures" });
            }
        }

        /// <summary>
        /// Get procedure by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Procedure>> GetProcedure(int id)
        {
            try
            {
                var procedure = await _procedureService.GetProcedureByIdAsync(id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedure not found" });
                }
                return Ok(procedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving procedure {id}");
                return StatusCode(500, new { message = "Error retrieving procedure" });
            }
        }

        /// <summary>
        /// Schedule procedure
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<Procedure>> ScheduleProcedure([FromBody] Procedure procedure)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdProcedure = await _procedureService.ScheduleProcedureAsync(procedure, userId);

                _logger.LogInformation($"Procedure scheduled: {createdProcedure.Id}");

                return CreatedAtAction(nameof(GetProcedure), new { id = createdProcedure.Id }, createdProcedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling procedure");
                return StatusCode(500, new { message = "Error scheduling procedure" });
            }
        }

        /// <summary>
        /// Update procedure status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateProcedureStatus(
            int id,
            [FromBody] string status)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _procedureService.UpdateProcedureStatusAsync(id, status, userId);

                _logger.LogInformation($"Procedure {id} status updated to {status}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Procedure not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating procedure {id}");
                return StatusCode(500, new { message = "Error updating procedure" });
            }
        }

        /// <summary>
        /// Record procedure outcome
        /// </summary>
        [HttpPut("{id}/outcome")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> RecordOutcome(
            int id,
            [FromBody] ProcedureOutcome outcome)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _procedureService.RecordOutcomeAsync(id, outcome.Outcome, outcome.Complications, outcome.FollowUpInstructions, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Procedure not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording outcome for procedure {id}");
                return StatusCode(500, new { message = "Error recording outcome" });
            }
        }

        /// <summary>
        /// Record consent
        /// </summary>
        [HttpPost("{id}/consent")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> RecordConsent(int id)
        {
            try
            {
                await _procedureService.RecordConsentAsync(id);

                _logger.LogInformation($"Consent recorded for procedure {id}");

                return Ok(new { message = "Consent recorded successfully" });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Procedure not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording consent for procedure {id}");
                return StatusCode(500, new { message = "Error recording consent" });
            }
        }

        /// <summary>
        /// Get scheduled procedures
        /// </summary>
        [HttpGet("scheduled")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Procedure>>> GetScheduledProcedures(
            [FromQuery] DateTime? date = null)
        {
            try
            {
                var procedures = await _procedureService.GetScheduledProceduresAsync(date);
                return Ok(procedures);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled procedures");
                return StatusCode(500, new { message = "Error retrieving scheduled procedures" });
            }
        }
    }

    public class ProcedureOutcome
    {
        public string? Outcome { get; set; }
        public string? Complications { get; set; }
        public string? FollowUpInstructions { get; set; }
    }
}