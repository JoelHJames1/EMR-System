using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Prescription
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public int MedicationId { get; set; }

        [Required]
        [StringLength(200)]
        public string Dosage { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Frequency { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string Route { get; set; } = string.Empty; // Oral, IV, IM, etc.

        [Required]
        public int Quantity { get; set; }

        [Required]
        public int Refills { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Discontinued, Completed, OnHold

        public bool IsGenericAllowed { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public Medication Medication { get; set; } = null!;
    }
}