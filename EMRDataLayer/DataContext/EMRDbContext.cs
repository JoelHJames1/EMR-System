using EMRDataLayer.Model;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace EMRDataLayer.DataContext
{
    public class EMRDBContext : IdentityDbContext<User>
    {
        // Core Entities
        public DbSet<User> Users { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Patient> Patients { get; set; }
        public DbSet<Provider> Providers { get; set; }

        // HL7 FHIR Clinical Entities
        public DbSet<Encounter> Encounters { get; set; }
        public DbSet<Diagnosis> Diagnoses { get; set; }
        public DbSet<Procedure> Procedures { get; set; }
        public DbSet<Observation> Observations { get; set; }
        public DbSet<ClinicalNote> ClinicalNotes { get; set; }
        public DbSet<CarePlan> CarePlans { get; set; }
        public DbSet<CarePlanActivity> CarePlanActivities { get; set; }
        public DbSet<Referral> Referrals { get; set; }
        public DbSet<FamilyHistory> FamilyHistories { get; set; }

        // Appointment & Scheduling
        public DbSet<Appointment> Appointments { get; set; }

        // Legacy Medical Records (consider migrating to Encounter/ClinicalNote)
        public DbSet<MedicalRecord> MedicalRecords { get; set; }
        public DbSet<VitalSign> VitalSigns { get; set; }
        public DbSet<Allergy> Allergies { get; set; }
        public DbSet<Immunization> Immunizations { get; set; }

        // Medication Entities
        public DbSet<Medication> Medications { get; set; }
        public DbSet<Prescription> Prescriptions { get; set; }

        // Lab Entities
        public DbSet<LabOrder> LabOrders { get; set; }
        public DbSet<LabResult> LabResults { get; set; }

        // Billing Entities
        public DbSet<Billing> Billings { get; set; }
        public DbSet<BillingItem> BillingItems { get; set; }
        public DbSet<Insurance> Insurances { get; set; }

        // Administrative & Facility
        public DbSet<Location> Locations { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Document> Documents { get; set; }

        public EMRDBContext(DbContextOptions<EMRDBContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Address configurations
            modelBuilder.Entity<Address>()
                .HasKey(a => a.Id);

            // User configurations
            modelBuilder.Entity<User>()
                .HasOne(u => u.Address)
                .WithMany(a => a.Users)
                .HasForeignKey(u => u.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Patient configurations
            modelBuilder.Entity<Patient>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Patient>()
                .HasOne(p => p.Address)
                .WithMany(a => a.Patients)
                .HasForeignKey(p => p.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Provider configurations
            modelBuilder.Entity<Provider>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Provider>()
                .HasOne(p => p.User)
                .WithOne(u => u.Provider)
                .HasForeignKey<Provider>(p => p.UserId)
                .OnDelete(DeleteBehavior.SetNull);

            // Appointment configurations
            modelBuilder.Entity<Appointment>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Provider)
                .WithMany(p => p.Appointments)
                .HasForeignKey(a => a.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // MedicalRecord configurations
            modelBuilder.Entity<MedicalRecord>()
                .HasKey(m => m.Id);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Patient)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<MedicalRecord>()
                .HasOne(m => m.Provider)
                .WithMany(p => p.MedicalRecords)
                .HasForeignKey(m => m.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // Prescription configurations
            modelBuilder.Entity<Prescription>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Patient)
                .WithMany(pa => pa.Prescriptions)
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Provider)
                .WithMany(pr => pr.Prescriptions)
                .HasForeignKey(p => p.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Prescription>()
                .HasOne(p => p.Medication)
                .WithMany(m => m.Prescriptions)
                .HasForeignKey(p => p.MedicationId)
                .OnDelete(DeleteBehavior.Restrict);

            // LabOrder configurations
            modelBuilder.Entity<LabOrder>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<LabOrder>()
                .HasOne(l => l.Patient)
                .WithMany(p => p.LabOrders)
                .HasForeignKey(l => l.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<LabOrder>()
                .HasOne(l => l.Provider)
                .WithMany(p => p.LabOrders)
                .HasForeignKey(l => l.ProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            // LabResult configurations
            modelBuilder.Entity<LabResult>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<LabResult>()
                .HasOne(l => l.LabOrder)
                .WithMany(o => o.LabResults)
                .HasForeignKey(l => l.LabOrderId)
                .OnDelete(DeleteBehavior.Cascade);

            // Billing configurations
            modelBuilder.Entity<Billing>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Patient)
                .WithMany(p => p.Billings)
                .HasForeignKey(b => b.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Billing>()
                .HasOne(b => b.Insurance)
                .WithMany(i => i.Billings)
                .HasForeignKey(b => b.InsuranceId)
                .OnDelete(DeleteBehavior.SetNull);

            // BillingItem configurations
            modelBuilder.Entity<BillingItem>()
                .HasKey(b => b.Id);

            modelBuilder.Entity<BillingItem>()
                .HasOne(b => b.Billing)
                .WithMany(bi => bi.BillingItems)
                .HasForeignKey(b => b.BillingId)
                .OnDelete(DeleteBehavior.Cascade);

            // Insurance configurations
            modelBuilder.Entity<Insurance>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Insurance>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Insurances)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // Allergy configurations
            modelBuilder.Entity<Allergy>()
                .HasKey(a => a.Id);

            modelBuilder.Entity<Allergy>()
                .HasOne(a => a.Patient)
                .WithMany(p => p.Allergies)
                .HasForeignKey(a => a.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Immunization configurations
            modelBuilder.Entity<Immunization>()
                .HasKey(i => i.Id);

            modelBuilder.Entity<Immunization>()
                .HasOne(i => i.Patient)
                .WithMany(p => p.Immunizations)
                .HasForeignKey(i => i.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // VitalSign configurations
            modelBuilder.Entity<VitalSign>()
                .HasKey(v => v.Id);

            modelBuilder.Entity<VitalSign>()
                .HasOne(v => v.Patient)
                .WithMany(p => p.VitalSigns)
                .HasForeignKey(v => v.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Medication configurations
            modelBuilder.Entity<Medication>()
                .HasKey(m => m.Id);

            // Encounter configurations
            modelBuilder.Entity<Encounter>()
                .HasKey(e => e.Id);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Patient)
                .WithMany()
                .HasForeignKey(e => e.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Provider)
                .WithMany()
                .HasForeignKey(e => e.ProviderId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Location)
                .WithMany(l => l.Encounters)
                .HasForeignKey(e => e.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Encounter>()
                .HasOne(e => e.Department)
                .WithMany(d => d.Encounters)
                .HasForeignKey(e => e.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            // Diagnosis configurations
            modelBuilder.Entity<Diagnosis>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Diagnosis>()
                .HasOne(d => d.Encounter)
                .WithMany(e => e.Diagnoses)
                .HasForeignKey(d => d.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);

            // Procedure configurations
            modelBuilder.Entity<Procedure>()
                .HasKey(p => p.Id);

            modelBuilder.Entity<Procedure>()
                .HasOne(p => p.Patient)
                .WithMany()
                .HasForeignKey(p => p.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Procedure>()
                .HasOne(p => p.Encounter)
                .WithMany(e => e.Procedures)
                .HasForeignKey(p => p.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Procedure>()
                .HasOne(p => p.Location)
                .WithMany(l => l.Procedures)
                .HasForeignKey(p => p.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Observation configurations
            modelBuilder.Entity<Observation>()
                .HasKey(o => o.Id);

            modelBuilder.Entity<Observation>()
                .HasOne(o => o.Patient)
                .WithMany()
                .HasForeignKey(o => o.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Observation>()
                .HasOne(o => o.Encounter)
                .WithMany(e => e.Observations)
                .HasForeignKey(o => o.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);

            // ClinicalNote configurations
            modelBuilder.Entity<ClinicalNote>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<ClinicalNote>()
                .HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<ClinicalNote>()
                .HasOne(c => c.Encounter)
                .WithMany()
                .HasForeignKey(c => c.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);

            // CarePlan configurations
            modelBuilder.Entity<CarePlan>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CarePlan>()
                .HasOne(c => c.Patient)
                .WithMany()
                .HasForeignKey(c => c.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            // CarePlanActivity configurations
            modelBuilder.Entity<CarePlanActivity>()
                .HasKey(c => c.Id);

            modelBuilder.Entity<CarePlanActivity>()
                .HasOne(c => c.CarePlan)
                .WithMany(cp => cp.Activities)
                .HasForeignKey(c => c.CarePlanId)
                .OnDelete(DeleteBehavior.Cascade);

            // Referral configurations
            modelBuilder.Entity<Referral>()
                .HasKey(r => r.Id);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.Patient)
                .WithMany()
                .HasForeignKey(r => r.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferringProvider)
                .WithMany()
                .HasForeignKey(r => r.ReferringProviderId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Referral>()
                .HasOne(r => r.ReferredToProvider)
                .WithMany()
                .HasForeignKey(r => r.ReferredToProviderId)
                .OnDelete(DeleteBehavior.SetNull);

            // FamilyHistory configurations
            modelBuilder.Entity<FamilyHistory>()
                .HasKey(f => f.Id);

            modelBuilder.Entity<FamilyHistory>()
                .HasOne(f => f.Patient)
                .WithMany()
                .HasForeignKey(f => f.PatientId)
                .OnDelete(DeleteBehavior.Cascade);

            // Location configurations
            modelBuilder.Entity<Location>()
                .HasKey(l => l.Id);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Department)
                .WithMany(d => d.Locations)
                .HasForeignKey(l => l.DepartmentId)
                .OnDelete(DeleteBehavior.SetNull);

            modelBuilder.Entity<Location>()
                .HasOne(l => l.Address)
                .WithMany()
                .HasForeignKey(l => l.AddressId)
                .OnDelete(DeleteBehavior.SetNull);

            // Department configurations
            modelBuilder.Entity<Department>()
                .HasKey(d => d.Id);

            // Document configurations
            modelBuilder.Entity<Document>()
                .HasKey(d => d.Id);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Patient)
                .WithMany()
                .HasForeignKey(d => d.PatientId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Document>()
                .HasOne(d => d.Encounter)
                .WithMany()
                .HasForeignKey(d => d.EncounterId)
                .OnDelete(DeleteBehavior.SetNull);

            // Appointment Location relationship
            modelBuilder.Entity<Appointment>()
                .HasOne(a => a.Location)
                .WithMany(l => l.Appointments)
                .HasForeignKey(a => a.LocationId)
                .OnDelete(DeleteBehavior.SetNull);

            // Seed default roles
            SeedRoles(modelBuilder);
        }

        private void SeedRoles(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<IdentityRole>().HasData(
                new IdentityRole
                {
                    Id = "1",
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR"
                },
                new IdentityRole
                {
                    Id = "2",
                    Name = "Doctor",
                    NormalizedName = "DOCTOR"
                },
                new IdentityRole
                {
                    Id = "3",
                    Name = "Nurse",
                    NormalizedName = "NURSE"
                },
                new IdentityRole
                {
                    Id = "4",
                    Name = "Receptionist",
                    NormalizedName = "RECEPTIONIST"
                },
                new IdentityRole
                {
                    Id = "5",
                    Name = "Lab Technician",
                    NormalizedName = "LAB TECHNICIAN"
                },
                new IdentityRole
                {
                    Id = "6",
                    Name = "Billing Staff",
                    NormalizedName = "BILLING STAFF"
                }
            );
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configuration = new ConfigurationBuilder()
                    .SetBasePath(Directory.GetCurrentDirectory())
                    .AddJsonFile("secrets.json", optional: true, reloadOnChange: true)
                    .Build();

                string connectionString = configuration.GetConnectionString("DefaultConnection") ?? "";
                optionsBuilder.UseSqlServer(connectionString);
            }
        }
    }
}
