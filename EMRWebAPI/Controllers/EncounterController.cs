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
    public class EncounterController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<EncounterController> _logger;

        public EncounterController(EMRDBContext context, ILogger<EncounterController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all encounters
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Encounter>>> GetEncounters(
            [FromQuery] int? patientId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Encounters
                    .Include(e => e.Patient)
                    .Include(e => e.Provider)
                    .Include(e => e.Location)
                    .AsQueryable();

                if (patientId.HasValue)
                {
                    query = query.Where(e => e.PatientId == patientId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(e => e.Status == status);
                }

                var encounters = await query
                    .OrderByDescending(e => e.StartDate)
                    .ToListAsync();

                return Ok(encounters);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving encounters");
                return StatusCode(500, new { message = "Error retrieving encounters" });
            }
        }

        /// <summary>
        /// Get encounter by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Encounter>> GetEncounter(int id)
        {
            try
            {
                var encounter = await _context.Encounters
                    .Include(e => e.Patient)
                    .Include(e => e.Provider)
                    .Include(e => e.Location)
                    .Include(e => e.Diagnoses)
                    .Include(e => e.Procedures)
                    .Include(e => e.Observations)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (encounter == null)
                {
                    return NotFound(new { message = "Encounter not found" });
                }

                return Ok(encounter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving encounter {id}");
                return StatusCode(500, new { message = "Error retrieving encounter" });
            }
        }

        /// <summary>
        /// Create new encounter
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Encounter>> CreateEncounter([FromBody] Encounter encounter)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                encounter.CreatedDate = DateTime.UtcNow;
                encounter.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Encounters.Add(encounter);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Encounter created: {encounter.Id}");

                return CreatedAtAction(nameof(GetEncounter), new { id = encounter.Id }, encounter);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating encounter");
                return StatusCode(500, new { message = "Error creating encounter" });
            }
        }

        /// <summary>
        /// Update encounter
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateEncounter(int id, [FromBody] Encounter encounter)
        {
            try
            {
                if (id != encounter.Id)
                {
                    return BadRequest(new { message = "Encounter ID mismatch" });
                }

                var existingEncounter = await _context.Encounters.FindAsync(id);
                if (existingEncounter == null)
                {
                    return NotFound(new { message = "Encounter not found" });
                }

                existingEncounter.Status = encounter.Status;
                existingEncounter.EndDate = encounter.EndDate;
                existingEncounter.DischargeDate = encounter.DischargeDate;
                existingEncounter.DischargeDisposition = encounter.DischargeDisposition;
                existingEncounter.ModifiedDate = DateTime.UtcNow;
                existingEncounter.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Encounter updated: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating encounter {id}");
                return StatusCode(500, new { message = "Error updating encounter" });
            }
        }

        /// <summary>
        /// Get encounter diagnoses
        /// </summary>
        [HttpGet("{id}/diagnoses")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Diagnosis>>> GetEncounterDiagnoses(int id)
        {
            try
            {
                var diagnoses = await _context.Diagnoses
                    .Where(d => d.EncounterId == id)
                    .OrderBy(d => d.Rank)
                    .ToListAsync();

                return Ok(diagnoses);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving diagnoses for encounter {id}");
                return StatusCode(500, new { message = "Error retrieving diagnoses" });
            }
        }
    }
}