using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IPatientService
    {
        Task<IEnumerable<Patient>> GetAllPatientsAsync(int page, int pageSize, string? search);
        Task<Patient?> GetPatientByIdAsync(int id);
        Task<Patient> CreatePatientAsync(Patient patient);
        Task<Patient> UpdatePatientAsync(int id, Patient patient);
        Task<bool> DeletePatientAsync(int id);
        Task<int> GetTotalPatientsCountAsync(string? search);
    }
}