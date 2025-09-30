using EMRDataLayer.Model;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository.IRepository
{
    // Diagnosis Repository
    public interface IDiagnosisRepository : IRepository<Diagnosis>
    {
        Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId);
    }

    // Procedure Repository
    public interface IProcedureRepository : IRepository<Procedure>
    {
        Task<IEnumerable<Procedure>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Procedure>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<Procedure>> GetScheduledProceduresAsync();
    }

    // Prescription Repository
    public interface IPrescriptionRepository : IRepository<Prescription>
    {
        Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId);
        Task<Prescription> GetByIdWithMedicationAsync(int id);
    }

    // Medication Repository
    public interface IMedicationRepository : IRepository<Medication>
    {
        Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm);
        Task<Medication> GetByNDCAsync(string ndc);
        Task<IEnumerable<Medication>> GetControlledSubstancesAsync();
    }

    // Lab Order Repository
    public interface ILabOrderRepository : IRepository<LabOrder>
    {
        Task<IEnumerable<LabOrder>> GetByPatientIdAsync(int patientId);
        Task<LabOrder> GetByIdWithResultsAsync(int id);
        Task<IEnumerable<LabOrder>> GetPendingOrdersAsync();
    }

    // Lab Result Repository
    public interface ILabResultRepository : IRepository<LabResult>
    {
        Task<IEnumerable<LabResult>> GetByLabOrderIdAsync(int labOrderId);
        Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId);
    }

    // Allergy Repository
    public interface IAllergyRepository : IRepository<Allergy>
    {
        Task<IEnumerable<Allergy>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId);
    }

    // Immunization Repository
    public interface IImmunizationRepository : IRepository<Immunization>
    {
        Task<IEnumerable<Immunization>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId);
    }

    // Observation (Vitals) Repository
    public interface IObservationRepository : IRepository<Observation>
    {
        Task<IEnumerable<Observation>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId);
        Task<Observation> GetLatestVitalsAsync(int patientId);
    }

    // Clinical Note Repository
    public interface IClinicalNoteRepository : IRepository<ClinicalNote>
    {
        Task<IEnumerable<ClinicalNote>> GetByEncounterIdAsync(int encounterId);
        Task<IEnumerable<ClinicalNote>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<ClinicalNote>> GetUnsignedNotesAsync(int providerId);
    }

    // Care Plan Repository
    public interface ICarePlanRepository : IRepository<CarePlan>
    {
        Task<IEnumerable<CarePlan>> GetByPatientIdAsync(int patientId);
        Task<CarePlan> GetByIdWithActivitiesAsync(int id);
        Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId);
    }

    // Referral Repository
    public interface IReferralRepository : IRepository<Referral>
    {
        Task<IEnumerable<Referral>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Referral>> GetPendingReferralsAsync();
        Task<IEnumerable<Referral>> GetByReferringProviderIdAsync(int providerId);
    }

    // Provider Repository
    public interface IProviderRepository : IRepository<Provider>
    {
        Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization);
        Task<Provider> GetByNPIAsync(string npiNumber);
        Task<IEnumerable<Provider>> GetActiveProvidersAsync();
    }

    // Billing Repository
    public interface IBillingRepository : IRepository<Billing>
    {
        Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId);
        Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync();
        Task<decimal> GetPatientBalanceAsync(int patientId);
    }

    // Insurance Repository
    public interface IInsuranceRepository : IRepository<Insurance>
    {
        Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId);
        Task<Insurance> GetActiveInsuranceAsync(int patientId);
    }
}