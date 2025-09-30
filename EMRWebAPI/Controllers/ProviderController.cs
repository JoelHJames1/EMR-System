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
    public class ProviderController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(EMRDBContext context, ILogger<ProviderController> logger)
        {
            _context = context;
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
                var query = _context.Providers
                    .Include(p => p.User)
                    .AsQueryable();

                if (!string.IsNullOrEmpty(specialization))
                {
                    query = query.Where(p => p.Specialization.Contains(specialization));
                }

                if (activeOnly)
                {
                    query = query.Where(p => p.IsActive);
                }

                var providers = await query
                    .OrderBy(p => p.LastName)
                    .ToListAsync();

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
                var provider = await _context.Providers
                    .Include(p => p.User)
                    .FirstOrDefaultAsync(p => p.Id == id);

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

                // Check if license number already exists
                var existingProvider = await _context.Providers
                    .FirstOrDefaultAsync(p => p.LicenseNumber == provider.LicenseNumber);

                if (existingProvider != null)
                {
                    return BadRequest(new { message = "License number already exists" });
                }

                provider.CreatedDate = DateTime.UtcNow;

                _context.Providers.Add(provider);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Provider created: {provider.Id}");

                return CreatedAtAction(nameof(GetProvider), new { id = provider.Id }, provider);
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

                var existingProvider = await _context.Providers.FindAsync(id);
                if (existingProvider == null)
                {
                    return NotFound(new { message = "Provider not found" });
                }

                existingProvider.FirstName = provider.FirstName;
                existingProvider.LastName = provider.LastName;
                existingProvider.MiddleName = provider.MiddleName;
                existingProvider.Specialization = provider.Specialization;
                existingProvider.Email = provider.Email;
                existingProvider.PhoneNumber = provider.PhoneNumber;
                existingProvider.NPI = provider.NPI;
                existingProvider.DEA = provider.DEA;
                existingProvider.ModifiedDate = DateTime.UtcNow;

                await _context.SaveChangesAsync();

                return NoContent();
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
                var query = _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Location)
                    .Where(a => a.ProviderId == id);

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate <= endDate.Value);
                }

                var appointments = await query
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

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
                var totalPatients = await _context.Encounters
                    .Where(e => e.ProviderId == id)
                    .Select(e => e.PatientId)
                    .Distinct()
                    .CountAsync();

                var totalEncounters = await _context.Encounters
                    .CountAsync(e => e.ProviderId == id);

                var totalPrescriptions = await _context.Prescriptions
                    .CountAsync(p => p.ProviderId == id);

                var totalLabOrders = await _context.LabOrders
                    .CountAsync(l => l.ProviderId == id);

                var upcomingAppointments = await _context.Appointments
                    .CountAsync(a => a.ProviderId == id &&
                                    a.AppointmentDate >= DateTime.Today &&
                                    a.Status != "Cancelled");

                return Ok(new
                {
                    totalPatients,
                    totalEncounters,
                    totalPrescriptions,
                    totalLabOrders,
                    upcomingAppointments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving statistics for provider {id}");
                return StatusCode(500, new { message = "Error retrieving statistics" });
            }
        }
    }
}