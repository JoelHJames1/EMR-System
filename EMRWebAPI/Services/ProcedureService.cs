using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using EMRWebAPI.Services.IServices;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services
{
    public class ProcedureService : IProcedureService
    {
        private readonly IProcedureRepository _procedureRepository;
        private readonly ILogger<ProcedureService> _logger;

        public ProcedureService(IProcedureRepository procedureRepository, ILogger<ProcedureService> logger)
        {
            _procedureRepository = procedureRepository;
            _logger = logger;
        }

        public async Task<IEnumerable<Procedure>> GetPatientProceduresAsync(int patientId)
        {
            try
            {
                return await _procedureRepository.GetByPatientIdAsync(patientId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving procedures for patient {patientId}");
                throw;
            }
        }

        public async Task<Procedure?> GetProcedureByIdAsync(int id)
        {
            try
            {
                return await _procedureRepository.GetByIdAsync(id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving procedure {id}");
                throw;
            }
        }

        public async Task<IEnumerable<Procedure>> GetScheduledProceduresAsync(DateTime? date)
        {
            try
            {
                return await _procedureRepository.GetScheduledProceduresAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving scheduled procedures");
                throw;
            }
        }

        public async Task<Procedure> ScheduleProcedureAsync(Procedure procedure, string userId)
        {
            try
            {
                procedure.Status = "Scheduled";
                procedure.CreatedDate = DateTime.UtcNow;
                procedure.CreatedBy = userId;

                return await _procedureRepository.AddAsync(procedure);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error scheduling procedure");
                throw;
            }
        }

        public async Task<bool> UpdateProcedureStatusAsync(int id, string status, string userId)
        {
            try
            {
                var procedure = await _procedureRepository.GetByIdAsync(id);
                if (procedure == null)
                {
                    return false;
                }

                procedure.Status = status;
                procedure.ModifiedDate = DateTime.UtcNow;
                procedure.ModifiedBy = userId;

                if (status == "InProgress" && !procedure.StartTime.HasValue)
                {
                    procedure.StartTime = DateTime.UtcNow;
                }
                else if (status == "Completed")
                {
                    procedure.EndTime = DateTime.UtcNow;
                    procedure.PerformedDate = DateTime.UtcNow;

                    if (procedure.StartTime.HasValue)
                    {
                        procedure.DurationMinutes = (int)(procedure.EndTime.Value - procedure.StartTime.Value).TotalMinutes;
                    }
                }

                await _procedureRepository.UpdateAsync(procedure);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating procedure status {id}");
                throw;
            }
        }

        public async Task<bool> RecordOutcomeAsync(int id, string outcome, string? complications, string? followUpInstructions, string userId)
        {
            try
            {
                var procedure = await _procedureRepository.GetByIdAsync(id);
                if (procedure == null)
                {
                    return false;
                }

                procedure.Outcome = outcome;
                procedure.Complications = complications;
                procedure.FollowUpInstructions = followUpInstructions;
                procedure.Status = "Completed";
                procedure.ModifiedDate = DateTime.UtcNow;
                procedure.ModifiedBy = userId;

                await _procedureRepository.UpdateAsync(procedure);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording outcome for procedure {id}");
                throw;
            }
        }

        public async Task<bool> RecordConsentAsync(int id)
        {
            try
            {
                var procedure = await _procedureRepository.GetByIdAsync(id);
                if (procedure == null)
                {
                    return false;
                }

                procedure.ConsentObtained = true;
                procedure.ConsentDate = DateTime.UtcNow;

                await _procedureRepository.UpdateAsync(procedure);
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error recording consent for procedure {id}");
                throw;
            }
        }
    }
}