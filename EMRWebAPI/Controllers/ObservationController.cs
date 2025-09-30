using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ObservationController : ControllerBase
    {
        private readonly IObservationService _observationService;
        private readonly ILogger<ObservationController> _logger;

        public ObservationController(IObservationService observationService, ILogger<ObservationController> logger)
        {
            _observationService = observationService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient observations
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Observation>>> GetPatientObservations(
            int patientId,
            [FromQuery] string? type = null)
        {
            try
            {
                var observations = await _observationService.GetPatientObservationsAsync(patientId, type);
                return Ok(observations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving observations for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving observations" });
            }
        }

        /// <summary>
        /// Create observation (LOINC coded)
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Observation>> CreateObservation([FromBody] Observation observation)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdObservation = await _observationService.CreateObservationAsync(observation, userId);

                _logger.LogInformation($"Observation created: {createdObservation.Id}");

                return Ok(createdObservation);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating observation");
                return StatusCode(500, new { message = "Error creating observation" });
            }
        }

        /// <summary>
        /// Record vital signs
        /// </summary>
        [HttpPost("vitals")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<VitalSign>> RecordVitalSigns([FromBody] VitalSign vitalSign)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdVitalSign = await _observationService.RecordVitalSignsAsync(vitalSign);

                _logger.LogInformation($"Vital signs recorded for patient {createdVitalSign.PatientId}");

                return Ok(createdVitalSign);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording vital signs");
                return StatusCode(500, new { message = "Error recording vital signs" });
            }
        }

        /// <summary>
        /// Get vital signs trends
        /// </summary>
        [HttpGet("patient/{patientId}/vitals/trends")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<object>> GetVitalsTrends(
            int patientId,
            [FromQuery] int days = 30)
        {
            try
            {
                var trends = await _observationService.GetVitalsTrendsAsync(patientId, days);
                return Ok(trends);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vital trends for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving vital trends" });
            }
        }
    }
}