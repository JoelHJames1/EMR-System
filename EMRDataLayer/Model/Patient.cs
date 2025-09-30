using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    public class Patient
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? MiddleName { get; set; }

        [Required]
        public DateTime DateOfBirth { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; } = string.Empty; // Male, Female, Other

        [StringLength(20)]
        public string? SocialSecurityNumber { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? EmergencyContact { get; set; }

        [StringLength(100)]
        public string? EmergencyContactName { get; set; }

        [StringLength(50)]
        public string? BloodType { get; set; }

        [StringLength(20)]
        public string? MaritalStatus { get; set; }

        [StringLength(100)]
        public string? Occupation { get; set; }

        [StringLength(100)]
        public string? Employer { get; set; }

        [StringLength(50)]
        public string? PreferredLanguage { get; set; }

        [StringLength(100)]
        public string? Ethnicity { get; set; }

        [StringLength(100)]
        public string? Race { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public int? AddressId { get; set; }
        public Address? Address { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
        public ICollection<Billing> Billings { get; set; } = new List<Billing>();
        public ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();
        public ICollection<Allergy> Allergies { get; set; } = new List<Allergy>();
        public ICollection<Immunization> Immunizations { get; set; } = new List<Immunization>();
        public ICollection<VitalSign> VitalSigns { get; set; } = new List<VitalSign>();
    }
}