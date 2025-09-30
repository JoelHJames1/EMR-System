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
    public class ProcedureController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ProcedureController> _logger;

        public ProcedureController(EMRDBContext context, ILogger<ProcedureController> logger)
        {
            _context = context;
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
                var procedures = await _context.Procedures
                    .Include(p => p.Provider)
                    .Include(p => p.Location)
                    .Where(p => p.PatientId == patientId)
                    .OrderByDescending(p => p.PerformedDate ?? p.ScheduledDate)
                    .ToListAsync();

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
                var procedure = await _context.Procedures
                    .Include(p => p.Patient)
                    .Include(p => p.Provider)
                    .Include(p => p.Location)
                    .FirstOrDefaultAsync(p => p.Id == id);

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

                procedure.Status = "Scheduled";
                procedure.CreatedDate = DateTime.UtcNow;
                procedure.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Procedures.Add(procedure);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Procedure scheduled: {procedure.Id}");

                return CreatedAtAction(nameof(GetProcedure), new { id = procedure.Id }, procedure);
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
                var procedure = await _context.Procedures.FindAsync(id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedure not found" });
                }

                procedure.Status = status;
                procedure.ModifiedDate = DateTime.UtcNow;
                procedure.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (status == "InProgress" && !procedure.StartTime.HasValue)
                {
                    procedure.StartTime = DateTime.UtcNow;
                }
                else if (status == "Completed")
                {
                    procedure.EndTime = DateTime.UtcNow;
                    procedure.PerformedDate = DateTime.UtcNow;

                    if (procedure.StartTime.HasValue)
                    {
                        procedure.DurationMinutes = (int)(procedure.EndTime.Value - procedure.StartTime.Value).TotalMinutes;
                    }
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Procedure {id} status updated to {status}");

                return NoContent();
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
                var procedure = await _context.Procedures.FindAsync(id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedure not found" });
                }

                procedure.Outcome = outcome.Outcome;
                procedure.Complications = outcome.Complications;
                procedure.FollowUpInstructions = outcome.FollowUpInstructions;
                procedure.Status = "Completed";
                procedure.ModifiedDate = DateTime.UtcNow;
                procedure.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
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
                var procedure = await _context.Procedures.FindAsync(id);
                if (procedure == null)
                {
                    return NotFound(new { message = "Procedure not found" });
                }

                procedure.ConsentObtained = true;
                procedure.ConsentDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Consent recorded for procedure {id}");

                return Ok(new { message = "Consent recorded successfully" });
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
                var targetDate = date ?? DateTime.Today;

                var procedures = await _context.Procedures
                    .Include(p => p.Patient)
                    .Include(p => p.Provider)
                    .Include(p => p.Location)
                    .Where(p => p.Status == "Scheduled" &&
                               p.ScheduledDate.HasValue &&
                               p.ScheduledDate.Value.Date == targetDate.Date)
                    .OrderBy(p => p.ScheduledDate)
                    .ToListAsync();

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