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
    public class InsuranceController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<InsuranceController> _logger;

        public InsuranceController(EMRDBContext context, ILogger<InsuranceController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient insurance policies
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Billing Staff,Receptionist")]
        public async Task<ActionResult<IEnumerable<Insurance>>> GetPatientInsurance(int patientId)
        {
            try
            {
                var insurance = await _context.Insurances
                    .Where(i => i.PatientId == patientId && i.IsActive)
                    .OrderByDescending(i => i.IsPrimary)
                    .ThenByDescending(i => i.EffectiveDate)
                    .ToListAsync();

                return Ok(insurance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving insurance for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving insurance" });
            }
        }

        /// <summary>
        /// Add insurance policy
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Billing Staff,Receptionist")]
        public async Task<ActionResult<Insurance>> AddInsurance([FromBody] Insurance insurance)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // If this is primary, set others to secondary
                if (insurance.IsPrimary)
                {
                    var existingPrimary = await _context.Insurances
                        .Where(i => i.PatientId == insurance.PatientId && i.IsPrimary && i.IsActive)
                        .ToListAsync();

                    foreach (var ins in existingPrimary)
                    {
                        ins.IsPrimary = false;
                    }
                }

                insurance.CreatedDate = DateTime.UtcNow;
                insurance.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Insurances.Add(insurance);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Insurance added for patient {insurance.PatientId}");

                return Ok(insurance);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error adding insurance");
                return StatusCode(500, new { message = "Error adding insurance" });
            }
        }

        /// <summary>
        /// Update insurance
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<IActionResult> UpdateInsurance(int id, [FromBody] Insurance insurance)
        {
            try
            {
                if (id != insurance.Id)
                {
                    return BadRequest(new { message = "Insurance ID mismatch" });
                }

                var existingInsurance = await _context.Insurances.FindAsync(id);
                if (existingInsurance == null)
                {
                    return NotFound(new { message = "Insurance not found" });
                }

                existingInsurance.InsuranceCompany = insurance.InsuranceCompany;
                existingInsurance.PolicyNumber = insurance.PolicyNumber;
                existingInsurance.GroupNumber = insurance.GroupNumber;
                existingInsurance.EffectiveDate = insurance.EffectiveDate;
                existingInsurance.ExpirationDate = insurance.ExpirationDate;
                existingInsurance.ModifiedDate = DateTime.UtcNow;
                existingInsurance.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating insurance {id}");
                return StatusCode(500, new { message = "Error updating insurance" });
            }
        }

        /// <summary>
        /// Verify insurance eligibility
        /// </summary>
        [HttpGet("{id}/verify")]
        [Authorize(Roles = "Administrator,Billing Staff,Receptionist")]
        public async Task<ActionResult<object>> VerifyInsurance(int id)
        {
            try
            {
                var insurance = await _context.Insurances.FindAsync(id);
                if (insurance == null)
                {
                    return NotFound(new { message = "Insurance not found" });
                }

                // Mock verification - in real implementation, integrate with eligibility verification service
                var isActive = insurance.IsActive &&
                              insurance.EffectiveDate <= DateTime.Today &&
                              (!insurance.ExpirationDate.HasValue || insurance.ExpirationDate >= DateTime.Today);

                return Ok(new
                {
                    insurance.PolicyNumber,
                    insurance.InsuranceCompany,
                    isEligible = isActive,
                    effectiveDate = insurance.EffectiveDate,
                    expirationDate = insurance.ExpirationDate,
                    verifiedDate = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error verifying insurance {id}");
                return StatusCode(500, new { message = "Error verifying insurance" });
            }
        }

        /// <summary>
        /// Deactivate insurance
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<IActionResult> DeactivateInsurance(int id)
        {
            try
            {
                var insurance = await _context.Insurances.FindAsync(id);
                if (insurance == null)
                {
                    return NotFound(new { message = "Insurance not found" });
                }

                insurance.IsActive = false;
                insurance.ModifiedDate = DateTime.UtcNow;
                insurance.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating insurance {id}");
                return StatusCode(500, new { message = "Error deactivating insurance" });
            }
        }
    }
}