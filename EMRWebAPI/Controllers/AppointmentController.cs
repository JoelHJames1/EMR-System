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
    public class AppointmentController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(EMRDBContext context, ILogger<AppointmentController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get appointments by date range
        /// </summary>
        [HttpGet]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetAppointments(
            [FromQuery] DateTime? startDate = null,
            [FromQuery] DateTime? endDate = null,
            [FromQuery] int? providerId = null,
            [FromQuery] string? status = null)
        {
            try
            {
                var query = _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Provider)
                    .Include(a => a.Location)
                    .AsQueryable();

                if (startDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate >= startDate.Value);
                }

                if (endDate.HasValue)
                {
                    query = query.Where(a => a.AppointmentDate <= endDate.Value);
                }

                if (providerId.HasValue)
                {
                    query = query.Where(a => a.ProviderId == providerId.Value);
                }

                if (!string.IsNullOrEmpty(status))
                {
                    query = query.Where(a => a.Status == status);
                }

                var appointments = await query
                    .OrderBy(a => a.AppointmentDate)
                    .ThenBy(a => a.StartTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointments");
                return StatusCode(500, new { message = "Error retrieving appointments" });
            }
        }

        /// <summary>
        /// Get patient appointments
        /// </summary>
        [HttpGet("patient/{patientId}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetPatientAppointments(int patientId)
        {
            try
            {
                var appointments = await _context.Appointments
                    .Include(a => a.Provider)
                    .Include(a => a.Location)
                    .Where(a => a.PatientId == patientId)
                    .OrderByDescending(a => a.AppointmentDate)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointments for patient {patientId}");
                return StatusCode(500, new { message = "Error retrieving appointments" });
            }
        }

        /// <summary>
        /// Get appointment by ID
        /// </summary>
        [HttpGet("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Appointment>> GetAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Provider)
                    .Include(a => a.Location)
                    .FirstOrDefaultAsync(a => a.Id == id);

                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                return Ok(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointment {id}");
                return StatusCode(500, new { message = "Error retrieving appointment" });
            }
        }

        /// <summary>
        /// Create new appointment
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<Appointment>> CreateAppointment([FromBody] Appointment appointment)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // Check for scheduling conflicts
                var conflict = await _context.Appointments
                    .AnyAsync(a =>
                        a.ProviderId == appointment.ProviderId &&
                        a.AppointmentDate.Date == appointment.AppointmentDate.Date &&
                        a.Status != "Cancelled" &&
                        ((a.StartTime < appointment.EndTime && a.EndTime > appointment.StartTime)));

                if (conflict)
                {
                    return BadRequest(new { message = "Scheduling conflict detected" });
                }

                appointment.CreatedDate = DateTime.UtcNow;
                appointment.CreatedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                _context.Appointments.Add(appointment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"Appointment created: {appointment.Id}");

                return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                return StatusCode(500, new { message = "Error creating appointment" });
            }
        }

        /// <summary>
        /// Update appointment
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<IActionResult> UpdateAppointment(int id, [FromBody] Appointment appointment)
        {
            try
            {
                if (id != appointment.Id)
                {
                    return BadRequest(new { message = "Appointment ID mismatch" });
                }

                var existingAppointment = await _context.Appointments.FindAsync(id);
                if (existingAppointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                existingAppointment.Status = appointment.Status;
                existingAppointment.Notes = appointment.Notes;
                existingAppointment.ModifiedDate = DateTime.UtcNow;
                existingAppointment.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment {id}");
                return StatusCode(500, new { message = "Error updating appointment" });
            }
        }

        /// <summary>
        /// Cancel appointment
        /// </summary>
        [HttpPut("{id}/cancel")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<IActionResult> CancelAppointment(int id)
        {
            try
            {
                var appointment = await _context.Appointments.FindAsync(id);
                if (appointment == null)
                {
                    return NotFound(new { message = "Appointment not found" });
                }

                appointment.Status = "Cancelled";
                appointment.ModifiedDate = DateTime.UtcNow;
                appointment.ModifiedBy = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"Appointment cancelled: {id}");

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error cancelling appointment {id}");
                return StatusCode(500, new { message = "Error cancelling appointment" });
            }
        }

        /// <summary>
        /// Get today's appointments
        /// </summary>
        [HttpGet("today")]
        [Authorize(Roles = "Administrator,Doctor,Nurse,Receptionist")]
        public async Task<ActionResult<IEnumerable<Appointment>>> GetTodayAppointments()
        {
            try
            {
                var today = DateTime.Today;
                var appointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Provider)
                    .Where(a => a.AppointmentDate.Date == today && a.Status != "Cancelled")
                    .OrderBy(a => a.StartTime)
                    .ToListAsync();

                return Ok(appointments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving today's appointments");
                return StatusCode(500, new { message = "Error retrieving today's appointments" });
            }
        }
    }
}