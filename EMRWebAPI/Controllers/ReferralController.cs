using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ReferralController : ControllerBase
    {
        private readonly IReferralService _referralService;
        private readonly ILogger<ReferralController> _logger;

        public ReferralController(IReferralService referralService, ILogger<ReferralController> logger)
        {
            _referralService = referralService;
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
                var referrals = await _referralService.GetPatientReferralsAsync(patientId);
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
                var referral = await _referralService.GetReferralByIdAsync(id);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdReferral = await _referralService.CreateReferralAsync(referral, userId);

                _logger.LogInformation($"Referral created: {createdReferral.Id}");

                return CreatedAtAction(nameof(GetReferral), new { id = createdReferral.Id }, createdReferral);
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _referralService.UpdateReferralStatusAsync(id, status, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Referral not found" });
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
                var referrals = await _referralService.GetPendingReferralsAsync(providerId);
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