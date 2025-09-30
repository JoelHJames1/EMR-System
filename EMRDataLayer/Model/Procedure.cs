using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR Procedure - Medical/surgical procedures with CPT coding
    /// </summary>
    public class Procedure
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(20)]
        public string CPTCode { get; set; } = string.Empty;

        [StringLength(20)]
        public string? SNOMEDCode { get; set; }

        [Required]
        [StringLength(500)]
        public string ProcedureDescription { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Scheduled"; // Preparation, InProgress, Suspended, Aborted, Completed

        [StringLength(100)]
        public string? Category { get; set; } // Surgical, Diagnostic, Therapeutic

        [StringLength(500)]
        public string? ReasonForProcedure { get; set; }

        public DateTime? ScheduledDate { get; set; }

        public DateTime? PerformedDate { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int? DurationMinutes { get; set; }

        [StringLength(100)]
        public string? BodySite { get; set; }

        [StringLength(50)]
        public string? Laterality { get; set; } // Left, Right, Bilateral

        public string? Outcome { get; set; }

        public string? Complications { get; set; }

        public string? FollowUpInstructions { get; set; }

        public int? LocationId { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? EstimatedCost { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal? ActualCost { get; set; }

        public bool RequiresConsent { get; set; } = true;

        public bool ConsentObtained { get; set; } = false;

        public DateTime? ConsentDate { get; set; }

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
        public Location? Location { get; set; }
    }
}