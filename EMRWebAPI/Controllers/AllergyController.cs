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
    public class AllergyController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<AllergyController> _logger;

        public AllergyController(EMRDBContext context, ILogger<AllergyController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient allergies
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Allergy>>> GetPatientAllergies(int patientId)
        {
            try
            {
                var allergies = await _context.Allergies
                    .Where(a => a.PatientId == patientId && a.IsActive)
                    .OrderByDescending(a => a.Severity)
                    .ToListAsync();

                return Ok(allergies);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving allergies for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving allergies" });
            }
        }

        /// <summary>
        /// Add allergy
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Allergy>> AddAllergy([FromBody] Allergy allergy)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                allergy.CreatedDate = DateTime.UtcNow;
                allergy.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Allergies.Add(allergy);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Allergy added for patient {allergy.PatientId}: {allergy.Allergen}");

                return Ok(allergy);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding allergy");
                return StatusCode(500, new { message = "Error adding allergy" });
            }
        }

        /// <summary>
        /// Update allergy
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateAllergy(int id, [FromBody] Allergy allergy)
        {
            try
            {
                if (id != allergy.Id)
                {
                    return BadRequest(new { message = "Allergy ID mismatch" });
                }

                var existingAllergy = await _context.Allergies.FindAsync(id);
                if (existingAllergy == null)
                {
                    return NotFound(new { message = "Allergy not found" });
                }

                existingAllergy.Severity = allergy.Severity;
                existingAllergy.Reaction = allergy.Reaction;
                existingAllergy.Notes = allergy.Notes;
                existingAllergy.ModifiedDate = DateTime.UtcNow;
                existingAllergy.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating allergy {id}");
                return StatusCode(500, new { message = "Error updating allergy" });
            }
        }

        /// <summary>
        /// Deactivate allergy
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> DeactivateAllergy(int id)
        {
            try
            {
                var allergy = await _context.Allergies.FindAsync(id);
                if (allergy == null)
                {
                    return NotFound(new { message = "Allergy not found" });
                }

                allergy.IsActive = false;
                allergy.ModifiedDate = DateTime.UtcNow;
                allergy.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating allergy {id}");
                return StatusCode(500, new { message = "Error deactivating allergy" });
            }
        }
    }
}