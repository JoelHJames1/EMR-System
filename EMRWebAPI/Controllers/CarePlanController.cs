using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CarePlanController : ControllerBase
    {
        private readonly ICarePlanService _carePlanService;
        private readonly ILogger<CarePlanController> _logger;

        public CarePlanController(ICarePlanService carePlanService, ILogger<CarePlanController> logger)
        {
            _carePlanService = carePlanService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient care plans
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<CarePlan>>> GetPatientCarePlans(int patientId)
        {
            try
            {
                var carePlans = await _carePlanService.GetPatientCarePlansAsync(patientId);
                return Ok(carePlans);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving care plans for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving care plans" });
            }
        }

        /// <summary>
        /// Get care plan by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<CarePlan>> GetCarePlan(int id)
        {
            try
            {
                var carePlan = await _carePlanService.GetCarePlanByIdAsync(id);
                if (carePlan == null)
                {
                    return NotFound(new { message = "Care plan not found" });
                }
                return Ok(carePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving care plan {id}");
                return StatusCode(500, new { message = "Error retrieving care plan" });
            }
        }

        /// <summary>
        /// Create care plan
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<CarePlan>> CreateCarePlan([FromBody] CarePlan carePlan)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdCarePlan = await _carePlanService.CreateCarePlanAsync(carePlan, userId);

                _logger.LogInformation($"Care plan created: {createdCarePlan.Id}");

                return CreatedAtAction(nameof(GetCarePlan), new { id = createdCarePlan.Id }, createdCarePlan);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating care plan");
                return StatusCode(500, new { message = "Error creating care plan" });
            }
        }

        /// <summary>
        /// Add activity to care plan
        /// </summary>
        [HttpPost("{id}/activities")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<CarePlanActivity>> AddActivity(
            int id,
            [FromBody] CarePlanActivity activity)
        {
            try
            {
                var addedActivity = await _carePlanService.AddActivityAsync(id, activity);
                return Ok(addedActivity);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Care plan not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding activity to care plan {id}");
                return StatusCode(500, new { message = "Error adding activity" });
            }
        }

        /// <summary>
        /// Update activity status
        /// </summary>
        [HttpPut("activities/{activityId}/status")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateActivityStatus(
            int activityId,
            [FromBody] string status)
        {
            try
            {
                await _carePlanService.UpdateActivityStatusAsync(activityId, status);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Activity not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating activity {activityId}");
                return StatusCode(500, new { message = "Error updating activity" });
            }
        }

        /// <summary>
        /// Update care plan status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<IActionResult> UpdateCarePlanStatus(
            int id,
            [FromBody] string status)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _carePlanService.UpdateCarePlanStatusAsync(id, status, userId);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Care plan not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating care plan {id}");
                return StatusCode(500, new { message = "Error updating care plan" });
            }
        }
    }
}