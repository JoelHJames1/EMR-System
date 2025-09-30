using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR DocumentReference - Clinical documents, reports, images
    /// </summary>
    public class Document
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        public int? ProviderId { get; set; }

        [Required]
        [StringLength(200)]
        public string DocumentTitle { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string DocumentType { get; set; } = string.Empty; // LabReport, Prescription, Imaging, Consent, Discharge

        [StringLength(50)]
        public string Status { get; set; } = "Current"; // Current, Superseded, EnteredInError

        [StringLength(100)]
        public string? Category { get; set; } // Clinical Note, Lab Report, Radiology, etc.

        [StringLength(50)]
        public string? MimeType { get; set; } // application/pdf, image/jpeg, etc.

        [StringLength(500)]
        public string? FilePath { get; set; }

        [StringLength(200)]
        public string? FileName { get; set; }

        public long? FileSize { get; set; } // in bytes

        [StringLength(100)]
        public string? FileHash { get; set; } // For integrity verification

        public DateTime DocumentDate { get; set; } = DateTime.UtcNow;

        public DateTime? IndexedDate { get; set; }

        [StringLength(1000)]
        public string? Description { get; set; }

        public string? Content { get; set; } // For text-based documents

        [StringLength(50)]
        public string? SecurityLabel { get; set; } // Normal, Restricted, VeryRestricted

        public bool IsConfidential { get; set; } = true;

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