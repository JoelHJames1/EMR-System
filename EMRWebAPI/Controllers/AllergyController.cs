using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AllergyController : ControllerBase
    {
        private readonly IAllergyService _allergyService;
        private readonly ILogger<AllergyController> _logger;

        public AllergyController(IAllergyService allergyService, ILogger<AllergyController> logger)
        {
            _allergyService = allergyService;
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
                var allergies = await _allergyService.GetPatientAllergiesAsync(patientId);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdAllergy = await _allergyService.AddAllergyAsync(allergy, userId);

                _logger.LogInformation($"Allergy added for patient {createdAllergy.PatientId}: {createdAllergy.Allergen}");

                return Ok(createdAllergy);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _allergyService.UpdateAllergyAsync(id, allergy, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Allergy not found" });
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _allergyService.DeactivateAllergyAsync(id, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Allergy not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating allergy {id}");
                return StatusCode(500, new { message = "Error deactivating allergy" });
            }
        }
    }
}