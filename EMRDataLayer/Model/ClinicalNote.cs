using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// Clinical notes and documentation following SOAP format
    /// </summary>
    public class ClinicalNote
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string NoteType { get; set; } = string.Empty; // Progress, SOAP, History, Physical, Operative, Discharge

        public DateTime NoteDate { get; set; } = DateTime.UtcNow;

        // SOAP Format
        public string? Subjective { get; set; } // Patient's description of symptoms

        public string? Objective { get; set; } // Measurable observations

        public string? Assessment { get; set; } // Diagnosis and evaluation

        public string? Plan { get; set; } // Treatment plan

        // Additional sections
        public string? ChiefComplaint { get; set; }

        public string? HistoryOfPresentIllness { get; set; }

        public string? ReviewOfSystems { get; set; }

        public string? PastMedicalHistory { get; set; }

        public string? FamilyHistory { get; set; }

        public string? SocialHistory { get; set; }

        public string? PhysicalExam { get; set; }

        public string? Medications { get; set; }

        public string? Allergies { get; set; }

        public string? LabResults { get; set; }

        public string? ImagingResults { get; set; }

        public string? Procedures { get; set; }

        public string? FollowUp { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "Draft"; // Draft, Signed, Amended, Corrected

        public bool IsSigned { get; set; } = false;

        public DateTime? SignedDate { get; set; }

        [StringLength(36)]
        public string? SignedBy { get; set; }

        public bool IsAddendum { get; set; } = false;

        public int? ParentNoteId { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Encounter? Encounter { get; set; }
        public Provider Provider { get; set; } = null!;
    }
}