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
    public class ObservationController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ObservationController> _logger;

        public ObservationController(EMRDBContext context, ILogger<ObservationController> logger)
        {
            _context = context;
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
                var query = _context.Observations
                    .Include(o => o.Provider)
                    .Where(o => o.PatientId == patientId);

                if (!string.IsNullOrEmpty(type))
                {
                    query = query.Where(o => o.ObservationType == type);
                }

                var observations = await query
                    .OrderByDescending(o => o.ObservationDateTime)
                    .Take(100)
                    .ToListAsync();

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

                observation.ObservationDateTime = DateTime.UtcNow;
                observation.CreatedDate = DateTime.UtcNow;
                observation.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Observations.Add(observation);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Observation created: {observation.Id}");

                return Ok(observation);
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

                vitalSign.MeasurementDate = DateTime.UtcNow;
                vitalSign.CreatedDate = DateTime.UtcNow;

                // Calculate BMI if height and weight provided
                if (vitalSign.Height.HasValue && vitalSign.Weight.HasValue)
                {
                    // Convert to metric if needed and calculate BMI
                    decimal heightMeters = vitalSign.HeightUnit == "in"
                        ? vitalSign.Height.Value * 0.0254m
                        : vitalSign.Height.Value / 100;

                    decimal weightKg = vitalSign.WeightUnit == "lbs"
                        ? vitalSign.Weight.Value * 0.453592m
                        : vitalSign.Weight.Value;

                    vitalSign.BMI = Math.Round(weightKg / (heightMeters * heightMeters), 2);
                }

                _context.VitalSigns.Add(vitalSign);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Vital signs recorded for patient {vitalSign.PatientId}");

                return Ok(vitalSign);
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
                var startDate = DateTime.UtcNow.AddDays(-days);

                var vitals = await _context.VitalSigns
                    .Where(v => v.PatientId == patientId && v.MeasurementDate >= startDate)
                    .OrderBy(v => v.MeasurementDate)
                    .Select(v => new
                    {
                        date = v.MeasurementDate,
                        temperature = v.Temperature,
                        systolic = v.SystolicBP,
                        diastolic = v.DiastolicBP,
                        heartRate = v.HeartRate,
                        oxygenSat = v.OxygenSaturation,
                        weight = v.Weight,
                        bmi = v.BMI
                    })
                    .ToListAsync();

                return Ok(new { patientId, days, data = vitals });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving vital trends for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving vital trends" });
            }
        }
    }
}