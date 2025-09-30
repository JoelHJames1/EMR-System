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

        // Clinical Entities
        public DbSet<Appointment> Appointments { get; set; }
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
