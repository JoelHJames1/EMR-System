using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class InsuranceController : ControllerBase
    {
        private readonly IInsuranceService _insuranceService;
        private readonly ILogger<InsuranceController> _logger;

        public InsuranceController(IInsuranceService insuranceService, ILogger<InsuranceController> logger)
        {
            _insuranceService = insuranceService;
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
                var insurance = await _insuranceService.GetPatientInsuranceAsync(patientId);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdInsurance = await _insuranceService.AddInsuranceAsync(insurance, userId);

                _logger.LogInformation($"Insurance added for patient {createdInsurance.PatientId}");

                return Ok(createdInsurance);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _insuranceService.UpdateInsuranceAsync(id, insurance, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Insurance not found" });
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
                var verificationResult = await _insuranceService.VerifyInsuranceAsync(id);
                return Ok(verificationResult);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Insurance not found" });
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _insuranceService.DeactivateInsuranceAsync(id, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Insurance not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deactivating insurance {id}");
                return StatusCode(500, new { message = "Error deactivating insurance" });
            }
        }
    }
}