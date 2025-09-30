using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IAppointmentService
    {
        Task<IEnumerable<Appointment>> GetAllAppointmentsAsync();
        Task<IEnumerable<Appointment>> GetPatientAppointmentsAsync(int patientId);
        Task<IEnumerable<Appointment>> GetProviderAppointmentsAsync(int providerId, DateTime date);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync();
        Task<Appointment?> GetAppointmentByIdAsync(int id);
        Task<Appointment> CreateAppointmentAsync(Appointment appointment, string userId);
        Task<Appointment> UpdateAppointmentAsync(int id, Appointment appointment, string userId);
        Task<bool> UpdateAppointmentStatusAsync(int id, string status, string userId);
        Task<bool> DeleteAppointmentAsync(int id);
    }
}