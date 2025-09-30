using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRWebAPI.Services.IServices
{
    public interface IProcedureService
    {
        Task<IEnumerable<Procedure>> GetPatientProceduresAsync(int patientId);
        Task<Procedure?> GetProcedureByIdAsync(int id);
        Task<IEnumerable<Procedure>> GetScheduledProceduresAsync(DateTime? date);
        Task<Procedure> ScheduleProcedureAsync(Procedure procedure, string userId);
        Task<bool> UpdateProcedureStatusAsync(int id, string status, string userId);
        Task<bool> RecordOutcomeAsync(int id, string outcome, string? complications, string? followUpInstructions, string userId);
        Task<bool> RecordConsentAsync(int id);
    }
}