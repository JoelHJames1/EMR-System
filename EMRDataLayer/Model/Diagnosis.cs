using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR Condition - Clinical diagnosis with ICD coding
    /// </summary>
    public class Diagnosis
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        public int? ProviderId { get; set; }

        [Required]
        [StringLength(10)]
        public string ICDCode { get; set; } = string.Empty; // ICD-10 or ICD-11

        [StringLength(10)]
        public string? ICDVersion { get; set; } = "ICD-10";

        [Required]
        [StringLength(500)]
        public string DiagnosisDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string DiagnosisType { get; set; } = "Primary"; // Primary, Secondary, Admitting, Discharge

        [StringLength(50)]
        public string ClinicalStatus { get; set; } = "Active"; // Active, Recurrence, Relapse, Inactive, Resolved

        [StringLength(50)]
        public string VerificationStatus { get; set; } = "Confirmed"; // Provisional, Differential, Confirmed, Refuted

        [StringLength(50)]
        public string? Severity { get; set; } // Mild, Moderate, Severe, Life-threatening

        public DateTime? OnsetDate { get; set; }

        public DateTime? AbatementDate { get; set; }

        public DateTime DiagnosisDate { get; set; } = DateTime.UtcNow;

        [StringLength(1000)]
        public string? Notes { get; set; }

        [Range(1, 10)]
        public int? Rank { get; set; } // Diagnosis ranking/priority

        public bool IsChronicCondition { get; set; } = false;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Encounter? Encounter { get; set; }
        public Provider? Provider { get; set; }
    }
}