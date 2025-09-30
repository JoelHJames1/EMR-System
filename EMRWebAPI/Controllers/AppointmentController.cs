using EMRDataLayer.Model;
using EMRWebAPI.Services.IServices;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;
        private readonly ILogger<AppointmentController> _logger;

        public AppointmentController(IAppointmentService appointmentService, ILogger<AppointmentController> logger)
        {
            _appointmentService = appointmentService;
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
                var appointments = await _appointmentService.GetAppointmentsAsync(startDate, endDate, providerId, status);
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
                var appointments = await _appointmentService.GetPatientAppointmentsAsync(patientId);
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
                var appointment = await _appointmentService.GetAppointmentByIdAsync(id);
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                var createdAppointment = await _appointmentService.CreateAppointmentAsync(appointment, userId);

                _logger.LogInformation($"Appointment created: {createdAppointment.Id}");

                return CreatedAtAction(nameof(GetAppointment), new { id = createdAppointment.Id }, createdAppointment);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
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

                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _appointmentService.UpdateAppointmentAsync(id, appointment, userId);

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Appointment not found" });
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
                var userId = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value ?? "System";
                await _appointmentService.CancelAppointmentAsync(id, userId);

                _logger.LogInformation($"Appointment cancelled: {id}");

                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Appointment not found" });
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
                var appointments = await _appointmentService.GetTodayAppointmentsAsync();
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