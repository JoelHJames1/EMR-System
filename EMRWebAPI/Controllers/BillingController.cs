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
    public class BillingController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<BillingController> _logger;

        public BillingController(EMRDBContext context, ILogger<BillingController> logger)
        {
            _context = context;
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
                var billings = await _context.Billings
                    .Include(b => b.Insurance)
                    .Include(b => b.BillingItems)
                    .Where(b => b.PatientId == patientId)
                    .OrderByDescending(b => b.InvoiceDate)
                    .ToListAsync();

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
                var billing = await _context.Billings
                    .Include(b => b.Patient)
                    .Include(b => b.Insurance)
                    .Include(b => b.BillingItems)
                    .FirstOrDefaultAsync(b => b.Id == id);

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

                // Generate invoice number
                billing.InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{new Random().Next(1000, 9999)}";
                billing.InvoiceDate = DateTime.UtcNow;
                billing.BalanceAmount = billing.TotalAmount;
                billing.CreatedDate = DateTime.UtcNow;
                billing.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                // Calculate insurance coverage if insurance is provided
                if (billing.InsuranceId.HasValue)
                {
                    billing.PatientResponsibility = billing.TotalAmount - (billing.InsuranceCoverage ?? 0);
                    billing.BalanceAmount = billing.PatientResponsibility ?? billing.TotalAmount;
                }

                _context.Billings.Add(billing);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Billing created: {billing.Id}");

                return CreatedAtAction(nameof(GetBilling), new { id = billing.Id }, billing);
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
                var billing = await _context.Billings.FindAsync(id);
                if (billing == null)
                {
                    return NotFound(new { message = "Billing record not found" });
                }

                billing.PaidAmount += payment.Amount;
                billing.BalanceAmount = billing.TotalAmount - billing.PaidAmount;
                billing.PaymentMethod = payment.Method;
                billing.PaymentDate = DateTime.UtcNow;

                // Update status
                if (billing.BalanceAmount <= 0)
                {
                    billing.Status = "Paid";
                }
                else if (billing.PaidAmount > 0)
                {
                    billing.Status = "Partial";
                }

                billing.ModifiedDate = DateTime.UtcNow;
                billing.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Payment recorded for billing {id}: ${payment.Amount}");

                return Ok(new { message = "Payment recorded successfully", balance = billing.BalanceAmount });
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
                var outstanding = await _context.Billings
                    .Include(b => b.Patient)
                    .Where(b => b.BalanceAmount > 0)
                    .OrderByDescending(b => b.BalanceAmount)
                    .Select(b => new
                    {
                        b.Id,
                        b.InvoiceNumber,
                        PatientName = $"{b.Patient.FirstName} {b.Patient.LastName}",
                        b.InvoiceDate,
                        b.TotalAmount,
                        b.PaidAmount,
                        b.BalanceAmount,
                        b.Status
                    })
                    .ToListAsync();

                var totalOutstanding = outstanding.Sum(b => b.BalanceAmount);

                return Ok(new
                {
                    totalOutstanding,
                    count = outstanding.Count,
                    records = outstanding
                });
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