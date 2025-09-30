using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class MedicalRecord
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        [StringLength(200)]
        public string ChiefComplaint { get; set; } = string.Empty;

        public string? HistoryOfPresentIllness { get; set; }

        public string? PhysicalExamination { get; set; }

        public string? Assessment { get; set; }

        public string? Diagnosis { get; set; }

        public string? TreatmentPlan { get; set; }

        public string? ProgressNotes { get; set; }

        [StringLength(50)]
        public string RecordType { get; set; } = "Consultation"; // Consultation, Progress Note, Discharge Summary

        public bool IsSigned { get; set; } = false;

        public DateTime? SignedDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
    }
}