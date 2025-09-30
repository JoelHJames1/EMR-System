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
    public class CarePlanController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<CarePlanController> _logger;

        public CarePlanController(EMRDBContext context, ILogger<CarePlanController> logger)
        {
            _context = context;
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
                var carePlans = await _context.CarePlans
                    .Include(c => c.Provider)
                    .Include(c => c.Activities)
                    .Where(c => c.PatientId == patientId)
                    .OrderByDescending(c => c.CreatedDate)
                    .ToListAsync();

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
                var carePlan = await _context.CarePlans
                    .Include(c => c.Patient)
                    .Include(c => c.Provider)
                    .Include(c => c.Activities)
                    .FirstOrDefaultAsync(c => c.Id == id);

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

                carePlan.Status = "Active";
                carePlan.CreatedDate = DateTime.UtcNow;
                carePlan.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.CarePlans.Add(carePlan);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Care plan created: {carePlan.Id}");

                return CreatedAtAction(nameof(GetCarePlan), new { id = carePlan.Id }, carePlan);
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
                var carePlan = await _context.CarePlans.FindAsync(id);
                if (carePlan == null)
                {
                    return NotFound(new { message = "Care plan not found" });
                }

                activity.CarePlanId = id;
                activity.CreatedDate = DateTime.UtcNow;

                _context.CarePlanActivities.Add(activity);
                await _context.SaveChangesAsync();

                return Ok(activity);
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
                var activity = await _context.CarePlanActivities.FindAsync(activityId);
                if (activity == null)
                {
                    return NotFound(new { message = "Activity not found" });
                }

                activity.Status = status;

                if (status == "Completed")
                {
                    activity.CompletedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return NoContent();
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
                var carePlan = await _context.CarePlans.FindAsync(id);
                if (carePlan == null)
                {
                    return NotFound(new { message = "Care plan not found" });
                }

                carePlan.Status = status;
                carePlan.ModifiedDate = DateTime.UtcNow;
                carePlan.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (status == "Completed")
                {
                    carePlan.EndDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating care plan {id}");
                return StatusCode(500, new { message = "Error updating care plan" });
            }
        }
    }
}