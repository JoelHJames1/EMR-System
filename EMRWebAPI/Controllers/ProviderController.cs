using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ProviderController : ControllerBase
    {
        private readonly IProviderService _providerService;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(IProviderService providerService, ILogger<ProviderController> logger)
        {
            _providerService = providerService;
            _logger = logger;
        }

        /// <summary>
        /// Get all providers
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Provider>>> GetProviders(
            [FromQuery] string? specialization = null,
            [FromQuery] bool activeOnly = true)
        {
            try
            {
                var providers = await _providerService.GetProvidersAsync(specialization, activeOnly);
                return Ok(providers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving providers");
                return StatusCode(500, new { message = "Error retrieving providers" });
            }
        }

        /// <summary>
        /// Get provider by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Provider>> GetProvider(int id)
        {
            try
            {
                var provider = await _providerService.GetProviderByIdAsync(id);
                if (provider == null)
                {
                    return NotFound(new { message = "Provider not found" });
                }
                return Ok(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving provider {id}");
                return StatusCode(500, new { message = "Error retrieving provider" });
            }
        }

        /// <summary>
        /// Create new provider
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<Provider>> CreateProvider([FromBody] Provider provider)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var createdProvider = await _providerService.CreateProviderAsync(provider);

                _logger.LogInformation($"Provider created: {createdProvider.Id}");

                return CreatedAtAction(nameof(GetProvider), new { id = createdProvider.Id }, createdProvider);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating provider");
                return StatusCode(500, new { message = "Error creating provider" });
            }
        }

        /// <summary>
        /// Update provider
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator")]
        public async Task<IActionResult> UpdateProvider(int id, [FromBody] Provider provider)
        {
            try
            {
                if (id != provider.Id)
                {
                    return BadRequest(new { message = "Provider ID mismatch" });
                }

                await _providerService.UpdateProviderAsync(id, provider);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Provider not found" });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating provider {id}");
                return StatusCode(500, new { message = "Error updating provider" });
            }
        }

        /// <summary>
        /// Get provider's schedule
        /// </summary>
        [HttpGet("{id}/schedule")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetProviderSchedule(
            int id,
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null)
        {
            try
            {
                var appointments = await _providerService.GetProviderScheduleAsync(id, startDate, endDate);
                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving schedule for provider {id}");
                return StatusCode(500, new { message = "Error retrieving schedule" });
            }
        }

        /// <summary>
        /// Get provider statistics
        /// </summary>
        [HttpGet("{id}/statistics")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<object>> GetProviderStatistics(int id)
        {
            try
            {
                var statistics = await _providerService.GetProviderStatisticsAsync(id);
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving statistics for provider {id}");
                return StatusCode(500, new { message = "Error retrieving statistics" });
            }
        }
    }
}