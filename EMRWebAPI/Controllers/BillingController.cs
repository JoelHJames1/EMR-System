using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class BillingController : ControllerBase
    {
        private readonly IBillingService _billingService;
        private readonly ILogger<BillingController> _logger;

        public BillingController(IBillingService billingService, ILogger<BillingController> logger)
        {
            _billingService = billingService;
            _logger = logger;
        }

        /// <summary>
        /// Get patient billing records
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Billing Staff")]
        public async Task<ActionResult<IEnumerable<Billing>>> GetPatientBillings(int patientId)
        {
            try
            {
                var billings = await _billingService.GetPatientBillingsAsync(patientId);
                return Ok(billings);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving billings for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving billings" });
            }
        }

        /// <summary>
        /// Get billing by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Billing Staff")]
        public async Task<ActionResult<Billing>> GetBilling(int id)
        {
            try
            {
                var billing = await _billingService.GetBillingByIdAsync(id);
                if (billing == null)
                {
                    return NotFound(new { message = "Billing record not found" });
                }
                return Ok(billing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving billing {id}");
                return StatusCode(500, new { message = "Error retrieving billing" });
            }
        }

        /// <summary>
        /// Create new billing/invoice
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<ActionResult<Billing>> CreateBilling([FromBody] Billing billing)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdBilling = await _billingService.CreateBillingAsync(billing, userId);

                _logger.LogInformation($"Billing created: {createdBilling.Id}");

                return CreatedAtAction(nameof(GetBilling), new { id = createdBilling.Id }, createdBilling);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating billing");
                return StatusCode(500, new { message = "Error creating billing" });
            }
        }

        /// <summary>
        /// Record payment
        /// </summary>
        [HttpPost("{id}/payment")]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<IActionResult> RecordPayment(
            int id,
            [FromBody] PaymentRequest payment)
        {
            try
            {
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var balance = await _billingService.RecordPaymentAsync(id, payment.Amount, payment.Method, userId);

                _logger.LogInformation($"Payment recorded for billing {id}: ${payment.Amount}");

                return Ok(new { message = "Payment recorded successfully", balance });
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Billing record not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording payment for billing {id}");
                return StatusCode(500, new { message = "Error recording payment" });
            }
        }

        /// <summary>
        /// Get outstanding balances
        /// </summary>
        [HttpGet("outstanding")]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<ActionResult<object>> GetOutstandingBalances()
        {
            try
            {
                var outstanding = await _billingService.GetOutstandingBalancesAsync();
                return Ok(outstanding);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving outstanding balances");
                return StatusCode(500, new { message = "Error retrieving outstanding balances" });
            }
        }
    }

    public class PaymentRequest
    {
        public decimal Amount { get; set; }
        public string Method { get; set; } = string.Empty;
    }
}