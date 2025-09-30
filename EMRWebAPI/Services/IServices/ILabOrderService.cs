using EMRDataLayer.Model;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface ILabOrderService
    {
        Task<IEnumerable<LabOrder>> GetAllLabOrdersAsync();
        Task<IEnumerable<LabOrder>> GetPatientLabOrdersAsync(int patientId);
        Task<IEnumerable<LabOrder>> GetPendingOrdersAsync();
        Task<IEnumerable<LabOrder>> GetPendingLabOrdersAsync();
        Task<LabOrder?> GetLabOrderByIdAsync(int id);
        Task<LabOrder> CreateLabOrderAsync(LabOrder labOrder, string userId);
        Task<bool> UpdateLabOrderStatusAsync(int id, string status, string userId);
        Task<LabResult> AddLabResultAsync(int labOrderId, LabResult labResult);
        Task<LabResult> AddLabResultAsync(int labOrderId, LabResult labResult, string userId);
    }
}