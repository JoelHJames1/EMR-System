using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IPrescriptionService
    {
        Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(int patientId);
        Task<IEnumerable<Prescription>> GetPatientPrescriptionsAsync(int patientId, bool? activeOnly);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId);
        Task<Prescription?> GetPrescriptionByIdAsync(int id);
        Task<Prescription> CreatePrescriptionAsync(Prescription prescription, string userId);
        Task<Prescription> UpdatePrescriptionAsync(int id, Prescription prescription, string userId);
        Task<bool> UpdatePrescriptionStatusAsync(int id, string status, string userId);
        Task<bool> RefillPrescriptionAsync(int id, string userId);
        Task<bool> DiscontinuePrescriptionAsync(int id, string userId);
        Task<IEnumerable<object>> GetMedicationsAsync(string? search);
    }
}