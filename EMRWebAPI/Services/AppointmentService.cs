using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IAppointmentRepository _appointmentRepository;
        private readonly ILogger<AppointmentService> _logger;

        public AppointmentService(IAppointmentRepository appointmentRepository, ILogger<AppointmentService> logger)
        {
            _appointmentRepository = appointmentRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Appointment>> GetAllAppointmentsAsync()
        {
            try
            {
                return await _appointmentRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all appointments");
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId)
        {
            try
            {
                return await _appointmentRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointments for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetProviderAppointmentsAsync(int providerId, DateTime date)
        {
            try
            {
                return await _appointmentRepository.GetByProviderIdAsync(providerId, date);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointments for provider {providerId}");
                throw;
            }
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync()
        {
            try
            {
                return await _appointmentRepository.GetUpcomingAppointmentsAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving upcoming appointments");
                throw;
            }
        }

        public async Task<Appointment?> GetAppointmentByIdAsync(int id)
        {
            try
            {
                return await _appointmentRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving appointment {id}");
                throw;
            }
        }

        public async Task<Appointment> CreateAppointmentAsync(Appointment appointment, string userId)
        {
            try
            {
                appointment.Status = "Scheduled";
                appointment.CreatedDate = DateTime.UtcNow;
                appointment.CreatedBy = userId;

                return await _appointmentRepository.AddAsync(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating appointment");
                throw;
            }
        }

        public async Task<Appointment> UpdateAppointmentAsync(int id, Appointment appointment, string userId)
        {
            try
            {
                var existingAppointment = await _appointmentRepository.GetByIdAsync(id);
                if (existingAppointment == null)
                {
                    throw new KeyNotFoundException($"Appointment with ID {id} not found");
                }

                appointment.Id = id;
                appointment.ModifiedDate = DateTime.UtcNow;
                appointment.ModifiedBy = userId;

                return await _appointmentRepository.UpdateAsync(appointment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment {id}");
                throw;
            }
        }

        public async Task<bool> UpdateAppointmentStatusAsync(int id, string status, string userId)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return false;
                }

                appointment.Status = status;
                appointment.ModifiedDate = DateTime.UtcNow;
                appointment.ModifiedBy = userId;

                await _appointmentRepository.UpdateAsync(appointment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating appointment status {id}");
                throw;
            }
        }

        public async Task<bool> DeleteAppointmentAsync(int id)
        {
            try
            {
                var appointment = await _appointmentRepository.GetByIdAsync(id);
                if (appointment == null)
                {
                    return false;
                }

                await _appointmentRepository.DeleteAsync(appointment);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting appointment {id}");
                throw;
            }
        }
    }
}