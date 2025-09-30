using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR Procedure DTO - Medical/surgical procedures with CPT coding
    /// </summary>
    public class ProcedureDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters")]
        public string? ProviderName { get; set; }

        [Required(ErrorMessage = "CPT code is required")]
        [StringLength(20, ErrorMessage = "CPT code cannot exceed 20 characters")]
        public string CPTCode { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "SNOMED code cannot exceed 20 characters")]
        public string? SNOMEDCode { get; set; }

        [Required(ErrorMessage = "Procedure description is required")]
        [StringLength(500, ErrorMessage = "Procedure description cannot exceed 500 characters")]
        public string ProcedureDescription { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Scheduled";

        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string? Category { get; set; }

        [StringLength(500, ErrorMessage = "Reason for procedure cannot exceed 500 characters")]
        public string? ReasonForProcedure { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? ScheduledDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? PerformedDate { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? StartTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? EndTime { get; set; }

        [Range(0, 1440, ErrorMessage = "Duration minutes must be between 0 and 1440")]
        public int? DurationMinutes { get; set; }

        [StringLength(100, ErrorMessage = "Body site cannot exceed 100 characters")]
        public string? BodySite { get; set; }

        [StringLength(50, ErrorMessage = "Laterality cannot exceed 50 characters")]
        public string? Laterality { get; set; }

        public string? Outcome { get; set; }

        public string? Complications { get; set; }

        public string? FollowUpInstructions { get; set; }

        public int? LocationId { get; set; }

        public decimal? EstimatedCost { get; set; }

        public decimal? ActualCost { get; set; }

        public bool RequiresConsent { get; set; } = true;

        public bool ConsentObtained { get; set; } = false;

        [DataType(DataType.Date)]
        public DateTime? ConsentDate { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}