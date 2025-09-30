using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// Clinical notes and documentation following SOAP format
    /// </summary>
    public class ClinicalNoteDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Encounter ID is required")]
        public int EncounterId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        [Required(ErrorMessage = "Note type is required")]
        [StringLength(100, ErrorMessage = "Note type cannot exceed 100 characters")]
        public string NoteType { get; set; } = string.Empty;

        [DataType(DataType.DateTime)]
        public DateTime NoteDate { get; set; }

        public string? Subjective { get; set; }

        public string? Objective { get; set; }

        public string? Assessment { get; set; }

        public string? Plan { get; set; }

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

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Draft";

        public bool IsSigned { get; set; } = false;

        [DataType(DataType.DateTime)]
        public DateTime? SignedDate { get; set; }

        [StringLength(36, ErrorMessage = "Signed by cannot exceed 36 characters")]
        public string? SignedBy { get; set; }

        public bool IsAddendum { get; set; } = false;

        public int? ParentNoteId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}