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
    public class ReferralController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ReferralController> _logger;

        public ReferralController(EMRDBContext context, ILogger<ReferralController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient referrals
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Referral>>> GetPatientReferrals(int patientId)
        {
            try
            {
                var referrals = await _context.Referrals
                    .Include(r => r.ReferringProvider)
                    .Include(r => r.ReferredToProvider)
                    .Where(r => r.PatientId == patientId)
                    .OrderByDescending(r => r.ReferralDate)
                    .ToListAsync();

                return Ok(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving referrals for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving referrals" });
            }
        }

        /// <summary>
        /// Get referral by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Referral>> GetReferral(int id)
        {
            try
            {
                var referral = await _context.Referrals
                    .Include(r => r.Patient)
                    .Include(r => r.ReferringProvider)
                    .Include(r => r.ReferredToProvider)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (referral == null)
                {
                    return NotFound(new { message = "Referral not found" });
                }

                return Ok(referral);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving referral {id}");
                return StatusCode(500, new { message = "Error retrieving referral" });
            }
        }

        /// <summary>
        /// Create referral
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<Referral>> CreateReferral([FromBody] Referral referral)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Generate referral number
                referral.ReferralNumber = $"REF-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                referral.Status = "Requested";
                referral.ReferralDate = DateTime.UtcNow;
                referral.CreatedDate = DateTime.UtcNow;
                referral.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Referrals.Add(referral);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Referral created: {referral.Id}");

                return CreatedAtAction(nameof(GetReferral), new { id = referral.Id }, referral);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating referral");
                return StatusCode(500, new { message = "Error creating referral" });
            }
        }

        /// <summary>
        /// Update referral status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Doctor,Receptionist")]
        public async Task<IActionResult> UpdateReferralStatus(
            int id,
            [FromBody] string status)
        {
            try
            {
                var referral = await _context.Referrals.FindAsync(id);
                if (referral == null)
                {
                    return NotFound(new { message = "Referral not found" });
                }

                referral.Status = status;
                referral.ModifiedDate = DateTime.UtcNow;
                referral.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (status == "Completed")
                {
                    referral.CompletedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating referral {id}");
                return StatusCode(500, new { message = "Error updating referral" });
            }
        }

        /// <summary>
        /// Get pending referrals
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Administrator,Doctor,Receptionist")]
        public async Task<ActionResult<IEnumerable<Referral>>> GetPendingReferrals([FromQuery] int? providerId = null)
        {
            try
            {
                var query = _context.Referrals
                    .Include(r => r.Patient)
                    .Include(r => r.ReferringProvider)
                    .Include(r => r.ReferredToProvider)
                    .Where(r => r.Status == "Requested" || r.Status == "Active");

                if (providerId.HasValue)
                {
                    query = query.Where(r => r.ReferredToProviderId == providerId.Value);
                }

                var referrals = await query
                    .OrderBy(r => r.Priority)
                    .ThenByDescending(r => r.ReferralDate)
                    .ToListAsync();

                return Ok(referrals);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending referrals");
                return StatusCode(500, new { message = "Error retrieving pending referrals" });
            }
        }
    }
}