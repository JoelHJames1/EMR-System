using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Provider
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
        [StringLength(50)]
        public string Specialization { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string LicenseNumber { get; set; } = string.Empty;

        [StringLength(20)]
        public string? NPI { get; set; } // National Provider Identifier

        [StringLength(20)]
        public string? DEA { get; set; } // Drug Enforcement Administration number

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Phone]
        public string PhoneNumber { get; set; } = string.Empty;

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        [StringLength(450)]
        public string? UserId { get; set; }
        public User? User { get; set; }

        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<MedicalRecord> MedicalRecords { get; set; } = new List<MedicalRecord>();
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
        public ICollection<LabOrder> LabOrders { get; set; } = new List<LabOrder>();
    }
}