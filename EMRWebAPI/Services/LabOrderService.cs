using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class LabOrderService : ILabOrderService
    {
        private readonly ILabOrderRepository _labOrderRepository;
        private readonly ILabResultRepository _labResultRepository;
        private readonly ILogger<LabOrderService> _logger;

        public LabOrderService(
            ILabOrderRepository labOrderRepository,
            ILabResultRepository labResultRepository,
            ILogger<LabOrderService> logger)
        {
            _labOrderRepository = labOrderRepository;
            _labResultRepository = labResultRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<LabOrder>> GetAllLabOrdersAsync()
        {
            try
            {
                return await _labOrderRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all lab orders");
                throw;
            }
        }

        public async Task<IEnumerable<LabOrder>> GetPatientLabOrdersAsync(int patientId)
        {
            try
            {
                return await _labOrderRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving lab orders for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<LabOrder>> GetPendingOrdersAsync()
        {
            try
            {
                return await _labOrderRepository.GetPendingOrdersAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving pending lab orders");
                throw;
            }
        }

        public async Task<LabOrder?> GetLabOrderByIdAsync(int id)
        {
            try
            {
                return await _labOrderRepository.GetByIdWithResultsAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving lab order {id}");
                throw;
            }
        }

        public async Task<LabOrder> CreateLabOrderAsync(LabOrder labOrder, string userId)
        {
            try
            {
                labOrder.Status = "Ordered";
                labOrder.OrderDate = DateTime.UtcNow;
                labOrder.CreatedDate = DateTime.UtcNow;
                labOrder.CreatedBy = userId;

                return await _labOrderRepository.AddAsync(labOrder);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating lab order");
                throw;
            }
        }

        public async Task<bool> UpdateLabOrderStatusAsync(int id, string status, string userId)
        {
            try
            {
                var labOrder = await _labOrderRepository.GetByIdAsync(id);
                if (labOrder == null)
                {
                    return false;
                }

                labOrder.Status = status;
                labOrder.ModifiedDate = DateTime.UtcNow;
                labOrder.ModifiedBy = userId;

                if (status == "Collected")
                {
                    labOrder.CollectionDate = DateTime.UtcNow;
                }
                else if (status == "Completed")
                {
                    labOrder.CompletionDate = DateTime.UtcNow;
                }

                await _labOrderRepository.UpdateAsync(labOrder);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating lab order status {id}");
                throw;
            }
        }

        public async Task<LabResult> AddLabResultAsync(int labOrderId, LabResult labResult, string userId)
        {
            try
            {
                var labOrder = await _labOrderRepository.GetByIdAsync(labOrderId);
                if (labOrder == null)
                {
                    throw new KeyNotFoundException($"Lab order with ID {labOrderId} not found");
                }

                labResult.LabOrderId = labOrderId;
                labResult.ResultDate = DateTime.UtcNow;
                labResult.CreatedDate = DateTime.UtcNow;
                labResult.CreatedBy = userId;

                var result = await _labResultRepository.AddAsync(labResult);

                // Update lab order status
                labOrder.Status = "Completed";
                labOrder.CompletionDate = DateTime.UtcNow;
                await _labOrderRepository.UpdateAsync(labOrder);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error adding lab result to order {labOrderId}");
                throw;
            }
        }
    }
}