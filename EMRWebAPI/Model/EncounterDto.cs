using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR Encounter DTO - Interaction between patient and healthcare provider
    /// </summary>
    public class EncounterDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Encounter number is required")]
        [StringLength(50, ErrorMessage = "Encounter number cannot exceed 50 characters")]
        public string EncounterNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        public int? ProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters")]
        public string? ProviderName { get; set; }

        [Required(ErrorMessage = "Encounter class is required")]
        [StringLength(50, ErrorMessage = "Encounter class cannot exceed 50 characters")]
        public string EncounterClass { get; set; } = string.Empty;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Planned";

        [StringLength(100, ErrorMessage = "Encounter type cannot exceed 100 characters")]
        public string? EncounterType { get; set; }

        [StringLength(50, ErrorMessage = "Priority cannot exceed 50 characters")]
        public string? Priority { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndDate { get; set; }

        [StringLength(500, ErrorMessage = "Reason for visit cannot exceed 500 characters")]
        public string? ReasonForVisit { get; set; }

        public int? LocationId { get; set; }

        public int? DepartmentId { get; set; }

        [StringLength(50, ErrorMessage = "Admission source cannot exceed 50 characters")]
        public string? AdmissionSource { get; set; }

        [StringLength(50, ErrorMessage = "Discharge disposition cannot exceed 50 characters")]
        public string? DischargeDisposition { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? AdmissionDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? DischargeDate { get; set; }

        public int? ReferralId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}