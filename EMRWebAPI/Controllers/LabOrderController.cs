using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class LabOrderController : ControllerBase
    {
        private readonly ILabOrderService _labOrderService;
        private readonly ILogger<LabOrderController> _logger;

        public LabOrderController(ILabOrderService labOrderService, ILogger<LabOrderController> logger)
        {
            _labOrderService = labOrderService;
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
                var labOrders = await _labOrderService.GetPatientLabOrdersAsync(patientId);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdLabOrder = await _labOrderService.CreateLabOrderAsync(labOrder, userId);

                _logger.LogInformation($"Lab order created: {createdLabOrder.Id}");

                return CreatedAtAction(nameof(GetPatientLabOrders),
                    new { patientId = createdLabOrder.PatientId }, createdLabOrder);
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _labOrderService.UpdateLabOrderStatusAsync(id, status, userId);

                _logger.LogInformation($"Lab order {id} status updated to {status}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Lab order not found" });
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
                var addedResult = await _labOrderService.AddLabResultAsync(id, labResult);

                _logger.LogInformation($"Lab result added for order: {id}");

                return Ok(addedResult);
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Lab order not found" });
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
                var pendingOrders = await _labOrderService.GetPendingLabOrdersAsync();
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