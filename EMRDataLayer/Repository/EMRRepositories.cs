using EMRDataLayer.DataContext;
using EMRDataLayer.Model;
using EMRDataLayer.Repository.IRepository;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EMRDataLayer.Repository
{
    // Patient Repository
    public class PatientRepository : Repository<Patient>, IPatientRepository
    {
        public PatientRepository(EMRDBContext context) : base(context) { }

        public async Task<Patient> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Patients
                .Include(p => p.Address)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Patient>> SearchPatientsAsync(string searchTerm)
        {
            return await _context.Patients
                .Where(p => p.FirstName.Contains(searchTerm) ||
                           p.LastName.Contains(searchTerm) ||
                           p.Email.Contains(searchTerm))
                .ToListAsync();
        }

        public async Task<IEnumerable<Patient>> GetActivePatients()
        {
            return await _context.Patients
                .Where(p => p.IsActive)
                .ToListAsync();
        }

        public async Task<bool> PatientExists(string mrn)
        {
            return await _context.Patients.AnyAsync(p => p.Email == mrn);
        }
    }

    // Appointment Repository
    public class AppointmentRepository : Repository<Appointment>, IAppointmentRepository
    {
        public AppointmentRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Appointment>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Appointments
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetByProviderIdAsync(int providerId, DateTime date)
        {
            return await _context.Appointments
                .Where(a => a.ProviderId == providerId && a.AppointmentDate.Date == date.Date)
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Appointment>> GetUpcomingAppointmentsAsync()
        {
            return await _context.Appointments
                .Where(a => a.AppointmentDate >= DateTime.Now && a.Status == "Scheduled")
                .OrderBy(a => a.AppointmentDate)
                .ToListAsync();
        }
    }

    // Encounter Repository
    public class EncounterRepository : Repository<Encounter>, IEncounterRepository
    {
        public EncounterRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Encounter>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Encounters
                .Where(e => e.PatientId == patientId)
                .OrderByDescending(e => e.StartDate)
                .ToListAsync();
        }

        public async Task<Encounter> GetByIdWithDetailsAsync(int id)
        {
            return await _context.Encounters
                .Include(e => e.Diagnoses)
                .Include(e => e.Procedures)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<IEnumerable<Encounter>> GetActiveEncountersAsync()
        {
            return await _context.Encounters
                .Where(e => e.Status == "InProgress")
                .ToListAsync();
        }
    }

    // Diagnosis Repository
    public class DiagnosisRepository : Repository<Diagnosis>, IDiagnosisRepository
    {
        public DiagnosisRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Diagnosis>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Diagnoses
                .Where(d => d.PatientId == patientId)
                .OrderByDescending(d => d.DiagnosisDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Diagnoses
                .Where(d => d.EncounterId == encounterId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Diagnosis>> GetActiveDiagnosesAsync(int patientId)
        {
            return await _context.Diagnoses
                .Where(d => d.PatientId == patientId && d.ClinicalStatus == "Active")
                .ToListAsync();
        }
    }

    // Procedure Repository
    public class ProcedureRepository : Repository<Procedure>, IProcedureRepository
    {
        public ProcedureRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Procedure>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Procedures
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.PerformedDate ?? p.ScheduledDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Procedure>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Procedures
                .Where(p => p.EncounterId == encounterId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Procedure>> GetScheduledProceduresAsync()
        {
            return await _context.Procedures
                .Where(p => p.Status == "Scheduled" && p.ScheduledDate >= DateTime.Now)
                .OrderBy(p => p.ScheduledDate)
                .ToListAsync();
        }
    }

    // Prescription Repository
    public class PrescriptionRepository : Repository<Prescription>, IPrescriptionRepository
    {
        public PrescriptionRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Prescription>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .Where(p => p.PatientId == patientId)
                .OrderByDescending(p => p.StartDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Prescription>> GetActivePrescriptionsAsync(int patientId)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .Where(p => p.PatientId == patientId && p.Status == "Active")
                .ToListAsync();
        }

        public async Task<Prescription> GetByIdWithMedicationAsync(int id)
        {
            return await _context.Prescriptions
                .Include(p => p.Medication)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
    }

    // Medication Repository
    public class MedicationRepository : Repository<Medication>, IMedicationRepository
    {
        public MedicationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Medication>> SearchMedicationsAsync(string searchTerm)
        {
            return await _context.Medications
                .Where(m => m.GenericName.Contains(searchTerm) ||
                           m.BrandName.Contains(searchTerm) ||
                           (m.NDC != null && m.NDC.Contains(searchTerm)))
                .ToListAsync();
        }

        public async Task<Medication> GetByNDCAsync(string ndc)
        {
            return await _context.Medications
                .FirstOrDefaultAsync(m => m.NDC == ndc);
        }

        public async Task<IEnumerable<Medication>> GetControlledSubstancesAsync()
        {
            return await _context.Medications
                .Where(m => m.DEASchedule != null && m.DEASchedule != "None")
                .OrderBy(m => m.DEASchedule)
                .ToListAsync();
        }
    }

    // Lab Order Repository
    public class LabOrderRepository : Repository<LabOrder>, ILabOrderRepository
    {
        public LabOrderRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<LabOrder>> GetByPatientIdAsync(int patientId)
        {
            return await _context.LabOrders
                .Where(l => l.PatientId == patientId)
                .OrderByDescending(l => l.OrderedDate)
                .ToListAsync();
        }

        public async Task<LabOrder> GetByIdWithResultsAsync(int id)
        {
            return await _context.LabOrders
                .Include(l => l.LabResults)
                .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<LabOrder>> GetPendingOrdersAsync()
        {
            return await _context.LabOrders
                .Where(l => l.Status == "Ordered")
                .OrderBy(l => l.OrderedDate)
                .ToListAsync();
        }
    }

    // Lab Result Repository
    public class LabResultRepository : Repository<LabResult>, ILabResultRepository
    {
        public LabResultRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<LabResult>> GetByLabOrderIdAsync(int labOrderId)
        {
            return await _context.LabResults
                .Where(r => r.LabOrderId == labOrderId)
                .ToListAsync();
        }

        public async Task<IEnumerable<LabResult>> GetByPatientIdAsync(int patientId)
        {
            return await _context.LabResults
                .Include(r => r.LabOrder)
                .Where(r => r.LabOrder.PatientId == patientId)
                .OrderByDescending(r => r.LabOrder.OrderedDate)
                .ToListAsync();
        }
    }

    // Allergy Repository
    public class AllergyRepository : Repository<Allergy>, IAllergyRepository
    {
        public AllergyRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Allergy>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Allergies
                .Where(a => a.PatientId == patientId)
                .OrderByDescending(a => a.OnsetDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Allergy>> GetActiveAllergiesAsync(int patientId)
        {
            return await _context.Allergies
                .Where(a => a.PatientId == patientId && a.IsActive)
                .ToListAsync();
        }
    }

    // Immunization Repository
    public class ImmunizationRepository : Repository<Immunization>, IImmunizationRepository
    {
        public ImmunizationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Immunization>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Immunizations
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.AdministeredDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Immunization>> GetImmunizationHistoryAsync(int patientId)
        {
            return await _context.Immunizations
                .Where(i => i.PatientId == patientId)
                .OrderBy(i => i.AdministeredDate)
                .ToListAsync();
        }
    }

    // Observation (Vitals) Repository
    public class ObservationRepository : Repository<Observation>, IObservationRepository
    {
        public ObservationRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Observation>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Observations
                .Where(o => o.PatientId == patientId)
                .OrderByDescending(o => o.ObservationDateTime)
                .ToListAsync();
        }

        public async Task<IEnumerable<Observation>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.Observations
                .Where(o => o.EncounterId == encounterId)
                .OrderByDescending(o => o.ObservationDateTime)
                .ToListAsync();
        }

        public async Task<Observation> GetLatestVitalsAsync(int patientId)
        {
            return await _context.Observations
                .Where(o => o.PatientId == patientId && o.ObservationType == "Vital Signs")
                .OrderByDescending(o => o.ObservationDateTime)
                .FirstOrDefaultAsync();
        }
    }

    // Clinical Note Repository
    public class ClinicalNoteRepository : Repository<ClinicalNote>, IClinicalNoteRepository
    {
        public ClinicalNoteRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<ClinicalNote>> GetByEncounterIdAsync(int encounterId)
        {
            return await _context.ClinicalNotes
                .Where(n => n.EncounterId == encounterId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicalNote>> GetByPatientIdAsync(int patientId)
        {
            return await _context.ClinicalNotes
                .Include(n => n.Encounter)
                .Where(n => n.Encounter.PatientId == patientId)
                .OrderByDescending(n => n.CreatedDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<ClinicalNote>> GetUnsignedNotesAsync(int providerId)
        {
            return await _context.ClinicalNotes
                .Where(n => n.ProviderId == providerId && !n.IsSigned)
                .OrderBy(n => n.CreatedDate)
                .ToListAsync();
        }
    }

    // Care Plan Repository
    public class CarePlanRepository : Repository<CarePlan>, ICarePlanRepository
    {
        public CarePlanRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<CarePlan>> GetByPatientIdAsync(int patientId)
        {
            return await _context.CarePlans
                .Where(c => c.PatientId == patientId)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();
        }

        public async Task<CarePlan> GetByIdWithActivitiesAsync(int id)
        {
            return await _context.CarePlans
                .Include(c => c.Activities)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<IEnumerable<CarePlan>> GetActiveCarePlansAsync(int patientId)
        {
            return await _context.CarePlans
                .Where(c => c.PatientId == patientId && c.Status == "Active")
                .ToListAsync();
        }
    }

    // Referral Repository
    public class ReferralRepository : Repository<Referral>, IReferralRepository
    {
        public ReferralRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Referral>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Referrals
                .Where(r => r.PatientId == patientId)
                .OrderByDescending(r => r.ReferralDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetPendingReferralsAsync()
        {
            return await _context.Referrals
                .Where(r => r.Status == "Pending")
                .OrderBy(r => r.ReferralDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Referral>> GetByReferringProviderIdAsync(int providerId)
        {
            return await _context.Referrals
                .Where(r => r.ReferringProviderId == providerId)
                .OrderByDescending(r => r.ReferralDate)
                .ToListAsync();
        }
    }

    // Provider Repository
    public class ProviderRepository : Repository<Provider>, IProviderRepository
    {
        public ProviderRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Provider>> GetBySpecializationAsync(string specialization)
        {
            return await _context.Providers
                .Where(p => p.Specialization == specialization && p.IsActive)
                .ToListAsync();
        }

        public async Task<Provider> GetByNPIAsync(string npiNumber)
        {
            return await _context.Providers
                .FirstOrDefaultAsync(p => p.NPI == npiNumber);
        }

        public async Task<IEnumerable<Provider>> GetActiveProvidersAsync()
        {
            return await _context.Providers
                .Where(p => p.IsActive)
                .OrderBy(p => p.LastName)
                .ToListAsync();
        }
    }

    // Billing Repository
    public class BillingRepository : Repository<Billing>, IBillingRepository
    {
        public BillingRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Billing>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Billings
                .Include(b => b.BillingItems)
                .Where(b => b.PatientId == patientId)
                .OrderByDescending(b => b.InvoiceDate)
                .ToListAsync();
        }

        public async Task<IEnumerable<Billing>> GetUnpaidInvoicesAsync()
        {
            return await _context.Billings
                .Where(b => b.Status != "Paid" && b.BalanceAmount > 0)
                .OrderBy(b => b.DueDate)
                .ToListAsync();
        }

        public async Task<decimal> GetPatientBalanceAsync(int patientId)
        {
            return await _context.Billings
                .Where(b => b.PatientId == patientId && b.Status != "Paid")
                .SumAsync(b => b.BalanceAmount);
        }
    }

    // Insurance Repository
    public class InsuranceRepository : Repository<Insurance>, IInsuranceRepository
    {
        public InsuranceRepository(EMRDBContext context) : base(context) { }

        public async Task<IEnumerable<Insurance>> GetByPatientIdAsync(int patientId)
        {
            return await _context.Insurances
                .Where(i => i.PatientId == patientId)
                .OrderByDescending(i => i.EffectiveDate)
                .ToListAsync();
        }

        public async Task<Insurance> GetActiveInsuranceAsync(int patientId)
        {
            return await _context.Insurances
                .Where(i => i.PatientId == patientId &&
                           i.IsActive &&
                           (i.ExpirationDate == null || i.ExpirationDate > DateTime.Now))
                .FirstOrDefaultAsync();
        }
    }
}