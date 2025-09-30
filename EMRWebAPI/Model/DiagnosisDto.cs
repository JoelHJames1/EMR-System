using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR Condition DTO - Clinical diagnosis with ICD coding
    /// </summary>
    public class DiagnosisDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        public int? ProviderId { get; set; }

        [Required(ErrorMessage = "ICD code is required")]
        [StringLength(10, ErrorMessage = "ICD code cannot exceed 10 characters")]
        public string ICDCode { get; set; } = string.Empty;

        [StringLength(10, ErrorMessage = "ICD version cannot exceed 10 characters")]
        public string? ICDVersion { get; set; } = "ICD-10";

        [Required(ErrorMessage = "Diagnosis description is required")]
        [StringLength(500, ErrorMessage = "Diagnosis description cannot exceed 500 characters")]
        public string DiagnosisDescription { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Diagnosis type cannot exceed 50 characters")]
        public string DiagnosisType { get; set; } = "Primary";

        [StringLength(50, ErrorMessage = "Clinical status cannot exceed 50 characters")]
        public string ClinicalStatus { get; set; } = "Active";

        [StringLength(50, ErrorMessage = "Verification status cannot exceed 50 characters")]
        public string VerificationStatus { get; set; } = "Confirmed";

        [StringLength(50, ErrorMessage = "Severity cannot exceed 50 characters")]
        public string? Severity { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OnsetDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ResolvedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime DiagnosisDate { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [Range(1, 10, ErrorMessage = "Rank must be between 1 and 10")]
        public int? Rank { get; set; }

        public bool IsChronicCondition { get; set; } = false;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}