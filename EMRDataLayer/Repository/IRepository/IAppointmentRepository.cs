using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IAppointmentRepository : IRepository<Appointment>
    {
        Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Appointment>> GetByProviderIdAsync(int providerId, DateTime date);
        Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync();
    }
}