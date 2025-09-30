using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EncounterController : ControllerBase
    {
        private readonly IEncounterService _encounterService;
        private readonly ILogger<EncounterController> _logger;

        public EncounterController(IEncounterService encounterService, ILogger<EncounterController> logger)
        {
            _encounterService = encounterService;
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
                var encounters = await _encounterService.GetEncountersAsync(patientId, status);
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
                var encounter = await _encounterService.GetEncounterByIdAsync(id);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdEncounter = await _encounterService.CreateEncounterAsync(encounter, userId);

                _logger.LogInformation($"Encounter created: {createdEncounter.Id}");

                return CreatedAtAction(nameof(GetEncounter), new { id = createdEncounter.Id }, createdEncounter);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _encounterService.UpdateEncounterAsync(id, encounter, userId);

                _logger.LogInformation($"Encounter updated: {id}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Encounter not found" });
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
                var diagnoses = await _encounterService.GetEncounterDiagnosesAsync(id);
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