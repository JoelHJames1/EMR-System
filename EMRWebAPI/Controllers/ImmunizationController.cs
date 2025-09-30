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
    public class ImmunizationController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ImmunizationController> _logger;

        public ImmunizationController(EMRDBContext context, ILogger<ImmunizationController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient immunizations
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<IEnumerable<Immunization>>> GetPatientImmunizations(int patientId)
        {
            try
            {
                var immunizations = await _context.Immunizations
                    .Where(i => i.PatientId == patientId)
                    .OrderByDescending(i => i.AdministeredDate)
                    .ToListAsync();

                return Ok(immunizations);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunizations for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving immunizations" });
            }
        }

        /// <summary>
        /// Record immunization
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<Immunization>> RecordImmunization([FromBody] Immunization immunization)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                immunization.AdministeredDate = DateTime.UtcNow;
                immunization.CreatedDate = DateTime.UtcNow;
                immunization.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Immunizations.Add(immunization);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Immunization recorded for patient {immunization.PatientId}: {immunization.VaccineName}");

                return Ok(immunization);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recording immunization");
                return StatusCode(500, new { message = "Error recording immunization" });
            }
        }

        /// <summary>
        /// Update immunization
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<IActionResult> UpdateImmunization(int id, [FromBody] Immunization immunization)
        {
            try
            {
                if (id != immunization.Id)
                {
                    return BadRequest(new { message = "Immunization ID mismatch" });
                }

                var existingImmunization = await _context.Immunizations.FindAsync(id);
                if (existingImmunization == null)
                {
                    return NotFound(new { message = "Immunization not found" });
                }

                existingImmunization.LotNumber = immunization.LotNumber;
                existingImmunization.Manufacturer = immunization.Manufacturer;
                existingImmunization.Notes = immunization.Notes;
                existingImmunization.ModifiedDate = DateTime.UtcNow;
                existingImmunization.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating immunization {id}");
                return StatusCode(500, new { message = "Error updating immunization" });
            }
        }

        /// <summary>
        /// Get immunization history report
        /// </summary>
        [HttpGet("patient/{patientId}/history")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<object>> GetImmunizationHistory(int patientId)
        {
            try
            {
                var immunizations = await _context.Immunizations
                    .Where(i => i.PatientId == patientId)
                    .OrderBy(i => i.AdministeredDate)
                    .Select(i => new
                    {
                        i.VaccineName,
                        i.CVXCode,
                        i.AdministeredDate,
                        i.DoseNumber,
                        i.AdministeredBy,
                        i.Manufacturer
                    })
                    .ToListAsync();

                var patient = await _context.Patients.FindAsync(patientId);

                return Ok(new
                {
                    patient = new
                    {
                        patient?.FirstName,
                        patient?.LastName,
                        patient?.DateOfBirth
                    },
                    totalImmunizations = immunizations.Count,
                    immunizations
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving immunization history for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving immunization history" });
            }
        }
    }
}