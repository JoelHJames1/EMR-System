using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class BillingService : IBillingService
    {
        private readonly IBillingRepository _billingRepository;
        private readonly ILogger<BillingService> _logger;

        public BillingService(IBillingRepository billingRepository, ILogger<BillingService> logger)
        {
            _billingRepository = billingRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Billing>> GetAllBillingsAsync()
        {
            try
            {
                return await _billingRepository.GetAllAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all billings");
                throw;
            }
        }

        public async Task<IEnumerable<Billing>> GetPatientBillingsAsync(int patientId)
        {
            try
            {
                return await _billingRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving billings for patient {patientId}");
                throw;
            }
        }

        public async Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync()
        {
            try
            {
                return await _billingRepository.GetUnpaidInvoicesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving unpaid invoices");
                throw;
            }
        }

        public async Task<Billing?> GetBillingByIdAsync(int id)
        {
            try
            {
                return await _billingRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving billing {id}");
                throw;
            }
        }

        public async Task<decimal> GetPatientBalanceAsync(int patientId)
        {
            try
            {
                return await _billingRepository.GetPatientBalanceAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving balance for patient {patientId}");
                throw;
            }
        }

        public async Task<Billing> CreateBillingAsync(Billing billing, string userId)
        {
            try
            {
                billing.InvoiceDate = DateTime.UtcNow;
                billing.Status = "Pending";
                billing.CreatedDate = DateTime.UtcNow;
                billing.CreatedBy = userId;

                return await _billingRepository.AddAsync(billing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating billing");
                throw;
            }
        }

        public async Task<Billing> UpdateBillingAsync(int id, Billing billing, string userId)
        {
            try
            {
                var existingBilling = await _billingRepository.GetByIdAsync(id);
                if (existingBilling == null)
                {
                    throw new KeyNotFoundException($"Billing with ID {id} not found");
                }

                billing.Id = id;
                billing.ModifiedDate = DateTime.UtcNow;
                billing.ModifiedBy = userId;

                return await _billingRepository.UpdateAsync(billing);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating billing {id}");
                throw;
            }
        }

        public async Task<bool> RecordPaymentAsync(int id, decimal amount, string userId)
        {
            try
            {
                var billing = await _billingRepository.GetByIdAsync(id);
                if (billing == null)
                {
                    return false;
                }

                billing.PaidAmount += amount;
                billing.BalanceAmount = billing.TotalAmount - billing.PaidAmount;
                billing.ModifiedDate = DateTime.UtcNow;
                billing.ModifiedBy = userId;

                if (billing.PaidAmount >= billing.TotalAmount)
                {
                    billing.Status = "Paid";
                    billing.PaymentDate = DateTime.UtcNow;
                }
                else
                {
                    billing.Status = "Partial";
                }

                await _billingRepository.UpdateAsync(billing);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording payment for billing {id}");
                throw;
            }
        }

        public async Task<bool> RecordPaymentAsync(int id, decimal amount, string userId, string paymentMethod)
        {
            try
            {
                var billing = await _billingRepository.GetByIdAsync(id);
                if (billing == null)
                {
                    return false;
                }

                billing.PaidAmount += amount;
                billing.BalanceAmount = billing.TotalAmount - billing.PaidAmount;
                billing.PaymentMethod = paymentMethod;
                billing.ModifiedDate = DateTime.UtcNow;
                billing.ModifiedBy = userId;

                if (billing.PaidAmount >= billing.TotalAmount)
                {
                    billing.Status = "Paid";
                    billing.PaymentDate = DateTime.UtcNow;
                }
                else
                {
                    billing.Status = "Partial";
                }

                await _billingRepository.UpdateAsync(billing);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording payment for billing {id} with method {paymentMethod}");
                throw;
            }
        }

        public async Task<IEnumerable<Billing>> GetOutstandingBalancesAsync()
        {
            try
            {
                var allBillings = await _billingRepository.GetAllAsync();
                return allBillings.Where(b => b.BalanceAmount > 0 && b.Status != "Paid");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving outstanding balances");
                throw;
            }
        }
    }
}