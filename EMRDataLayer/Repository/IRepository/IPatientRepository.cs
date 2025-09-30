using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    public interface IPatientRepository : IRepository<Patient>
    {
        Task<Patient> GetByIdWithDetailsAsync(int id);
        Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm);
        Task<IEnumerable<Patient>> GetActivePatients();
        Task<bool> PatientExists(string mrn);
        Task<IEnumerable<Patient>> GetAllWithPaginationAsync(int page, int pageSize, string? search);
        Task<int> GetTotalCountAsync(string? search);
    }
}