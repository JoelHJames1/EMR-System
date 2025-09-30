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
    public class LabOrderController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<LabOrderController> _logger;

        public LabOrderController(EMRDBContext context, ILogger<LabOrderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get patient lab orders
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Lab Technician")]
        public async Task<ActionResult<IEnumerable<LabOrder>>> GetPatientLabOrders(int patientId)
        {
            try
            {
                var labOrders = await _context.LabOrders
                    .Include(l => l.Provider)
                    .Include(l => l.LabResults)
                    .Where(l => l.PatientId == patientId)
                    .OrderByDescending(l => l.OrderedDate)
                    .ToListAsync();

                return Ok(labOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving lab orders for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving lab orders" });
            }
        }

        /// <summary>
        /// Create new lab order
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<LabOrder>> CreateLabOrder([FromBody] LabOrder labOrder)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                labOrder.OrderedDate = DateTime.UtcNow;
                labOrder.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.LabOrders.Add(labOrder);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lab order created: {labOrder.Id}");

                return CreatedAtAction(nameof(GetPatientLabOrders),
                    new { patientId = labOrder.PatientId }, labOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lab order");
                return StatusCode(500, new { message = "Error creating lab order" });
            }
        }

        /// <summary>
        /// Update lab order status
        /// </summary>
        [HttpPut("{id}/status")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Lab Technician")]
        public async Task<IActionResult> UpdateLabOrderStatus(
            int id,
            [FromBody] string status)
        {
            try
            {
                var labOrder = await _context.LabOrders.FindAsync(id);
                if (labOrder == null)
                {
                    return NotFound(new { message = "Lab order not found" });
                }

                labOrder.Status = status;
                labOrder.ModifiedDate = DateTime.UtcNow;
                labOrder.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                if (status == "InProgress" && !labOrder.CollectedDate.HasValue)
                {
                    labOrder.CollectedDate = DateTime.UtcNow;
                }
                else if (status == "Completed")
                {
                    labOrder.CompletedDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lab order {id} status updated to {status}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating lab order {id}");
                return StatusCode(500, new { message = "Error updating lab order" });
            }
        }

        /// <summary>
        /// Add lab results
        /// </summary>
        [HttpPost("{id}/results")]
        [Authorize(Roles = "Administrator,Lab Technician")]
        public async Task<ActionResult<LabResult>> AddLabResult(
            int id,
            [FromBody] LabResult labResult)
        {
            try
            {
                var labOrder = await _context.LabOrders.FindAsync(id);
                if (labOrder == null)
                {
                    return NotFound(new { message = "Lab order not found" });
                }

                labResult.LabOrderId = id;
                labResult.CreatedDate = DateTime.UtcNow;
                labResult.ResultDate = DateTime.UtcNow;

                _context.LabResults.Add(labResult);

                // Update lab order status
                labOrder.Status = "Completed";
                labOrder.CompletedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Lab result added for order: {id}");

                return Ok(labResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding lab result for order {id}");
                return StatusCode(500, new { message = "Error adding lab result" });
            }
        }

        /// <summary>
        /// Get pending lab orders
        /// </summary>
        [HttpGet("pending")]
        [Authorize(Roles = "Administrator,Lab Technician")]
        public async Task<ActionResult<IEnumerable<LabOrder>>> GetPendingLabOrders()
        {
            try
            {
                var pendingOrders = await _context.LabOrders
                    .Include(l => l.Patient)
                    .Include(l => l.Provider)
                    .Where(l => l.Status == "Ordered" || l.Status == "InProgress")
                    .OrderBy(l => l.Priority)
                    .ThenBy(l => l.OrderedDate)
                    .ToListAsync();

                return Ok(pendingOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending lab orders");
                return StatusCode(500, new { message = "Error retrieving pending lab orders" });
            }
        }
    }
}