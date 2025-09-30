using EMRDataLayer.DataContext;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EMRWebAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DashboardController : ControllerBase
    {
        private readonly EMRDBContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(EMRDBContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get system statistics
        /// </summary>
        [HttpGet("statistics")]
        [Authorize(Roles = "Administrator,Doctor")]
        public async Task<ActionResult<object>> GetSystemStatistics()
        {
            try
            {
                var totalPatients = await _context.Patients.CountAsync(p => p.IsActive);
                var totalProviders = await _context.Providers.CountAsync(p => p.IsActive);
                var totalAppointmentsToday = await _context.Appointments
                    .CountAsync(a => a.AppointmentDate.Date == DateTime.Today && a.Status != "Cancelled");
                var pendingLabOrders = await _context.LabOrders.CountAsync(l => l.Status == "Ordered" || l.Status == "InProgress");
                var activePrescriptions = await _context.Prescriptions.CountAsync(p => p.Status == "Active");
                var totalEncounters = await _context.Encounters.CountAsync();
                var outstandingBalance = await _context.Billings
                    .Where(b => b.BalanceAmount > 0)
                    .SumAsync(b => b.BalanceAmount);

                return Ok(new
                {
                    totalPatients,
                    totalProviders,
                    todayAppointments = totalAppointmentsToday,
                    pendingLabOrders,
                    activePrescriptions,
                    totalEncounters,
                    outstandingBalance
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving system statistics");
                return StatusCode(500, new { message = "Error retrieving statistics" });
            }
        }

        /// <summary>
        /// Get recent activity
        /// </summary>
        [HttpGet("activity")]
        [Authorize(Roles = "Administrator,Doctor,Nurse")]
        public async Task<ActionResult<object>> GetRecentActivity([FromQuery] int hours = 24)
        {
            try
            {
                var since = DateTime.UtcNow.AddHours(-hours);

                var recentEncounters = await _context.Encounters
                    .Include(e => e.Patient)
                    .Include(e => e.Provider)
                    .Where(e => e.CreatedDate >= since)
                    .OrderByDescending(e => e.CreatedDate)
                    .Take(10)
                    .Select(e => new
                    {
                        type = "Encounter",
                        e.Id,
                        patientName = $"{e.Patient.FirstName} {e.Patient.LastName}",
                        providerName = e.Provider != null ? $"{e.Provider.FirstName} {e.Provider.LastName}" : null,
                        e.Status,
                        date = e.CreatedDate
                    })
                    .ToListAsync();

                var recentAppointments = await _context.Appointments
                    .Include(a => a.Patient)
                    .Include(a => a.Provider)
                    .Where(a => a.CreatedDate >= since)
                    .OrderByDescending(a => a.CreatedDate)
                    .Take(10)
                    .Select(a => new
                    {
                        type = "Appointment",
                        a.Id,
                        patientName = $"{a.Patient.FirstName} {a.Patient.LastName}",
                        providerName = $"{a.Provider.FirstName} {a.Provider.LastName}",
                        a.Status,
                        date = a.CreatedDate
                    })
                    .ToListAsync();

                return Ok(new
                {
                    hours,
                    encountersCount = recentEncounters.Count,
                    appointmentsCount = recentAppointments.Count,
                    recentEncounters,
                    recentAppointments
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving recent activity");
                return StatusCode(500, new { message = "Error retrieving activity" });
            }
        }

        /// <summary>
        /// Get appointment statistics
        /// </summary>
        [HttpGet("appointments/stats")]
        [Authorize(Roles = "Administrator,Doctor,Receptionist")]
        public async Task<ActionResult<object>> GetAppointmentStatistics([FromQuery] int days = 7)
        {
            try
            {
                var startDate = DateTime.Today.AddDays(-days);

                var appointments = await _context.Appointments
                    .Where(a => a.AppointmentDate >= startDate)
                    .GroupBy(a => a.Status)
                    .Select(g => new { status = g.Key, count = g.Count() })
                    .ToListAsync();

                var byDate = await _context.Appointments
                    .Where(a => a.AppointmentDate >= startDate)
                    .GroupBy(a => a.AppointmentDate.Date)
                    .Select(g => new { date = g.Key, count = g.Count() })
                    .OrderBy(x => x.date)
                    .ToListAsync();

                return Ok(new
                {
                    days,
                    byStatus = appointments,
                    byDate
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving appointment statistics");
                return StatusCode(500, new { message = "Error retrieving appointment statistics" });
            }
        }

        /// <summary>
        /// Get provider workload
        /// </summary>
        [HttpGet("providers/workload")]
        [Authorize(Roles = "Administrator")]
        public async Task<ActionResult<object>> GetProviderWorkload([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.Today.AddDays(-days);

                var workload = await _context.Providers
                    .Where(p => p.IsActive)
                    .Select(p => new
                    {
                        providerId = p.Id,
                        providerName = $"{p.FirstName} {p.LastName}",
                        p.Specialization,
                        encountersCount = _context.Encounters.Count(e => e.ProviderId == p.Id && e.StartDate >= startDate),
                        appointmentsCount = _context.Appointments.Count(a => a.ProviderId == p.Id && a.AppointmentDate >= startDate),
                        prescriptionsCount = _context.Prescriptions.Count(pr => pr.ProviderId == p.Id && pr.CreatedDate >= startDate)
                    })
                    .OrderByDescending(p => p.encountersCount)
                    .ToListAsync();

                return Ok(new { days, workload });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving provider workload");
                return StatusCode(500, new { message = "Error retrieving provider workload" });
            }
        }

        /// <summary>
        /// Get lab statistics
        /// </summary>
        [HttpGet("lab/stats")]
        [Authorize(Roles = "Administrator,Lab Technician")]
        public async Task<ActionResult<object>> GetLabStatistics()
        {
            try
            {
                var totalOrders = await _context.LabOrders.CountAsync();
                var pending = await _context.LabOrders.CountAsync(l => l.Status == "Ordered");
                var inProgress = await _context.LabOrders.CountAsync(l => l.Status == "InProgress");
                var completed = await _context.LabOrders.CountAsync(l => l.Status == "Completed");

                var byPriority = await _context.LabOrders
                    .Where(l => l.Status != "Completed" && l.Status != "Cancelled")
                    .GroupBy(l => l.Priority)
                    .Select(g => new { priority = g.Key, count = g.Count() })
                    .ToListAsync();

                return Ok(new
                {
                    totalOrders,
                    pending,
                    inProgress,
                    completed,
                    byPriority
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving lab statistics");
                return StatusCode(500, new { message = "Error retrieving lab statistics" });
            }
        }

        /// <summary>
        /// Get billing summary
        /// </summary>
        [HttpGet("billing/summary")]
        [Authorize(Roles = "Administrator,Billing Staff")]
        public async Task<ActionResult<object>> GetBillingSummary([FromQuery] int days = 30)
        {
            try
            {
                var startDate = DateTime.Today.AddDays(-days);

                var totalBilled = await _context.Billings
                    .Where(b => b.InvoiceDate >= startDate)
                    .SumAsync(b => b.TotalAmount);

                var totalPaid = await _context.Billings
                    .Where(b => b.InvoiceDate >= startDate)
                    .SumAsync(b => b.PaidAmount);

                var totalOutstanding = await _context.Billings
                    .Where(b => b.BalanceAmount > 0)
                    .SumAsync(b => b.BalanceAmount);

                var byStatus = await _context.Billings
                    .Where(b => b.InvoiceDate >= startDate)
                    .GroupBy(b => b.Status)
                    .Select(g => new { status = g.Key, count = g.Count(), total = g.Sum(b => b.TotalAmount) })
                    .ToListAsync();

                return Ok(new
                {
                    days,
                    totalBilled,
                    totalPaid,
                    totalOutstanding,
                    collectionRate = totalBilled > 0 ? (totalPaid / totalBilled * 100) : 0,
                    byStatus
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving billing summary");
                return StatusCode(500, new { message = "Error retrieving billing summary" });
            }
        }
    }
}