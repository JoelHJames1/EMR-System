using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR CarePlan - Comprehensive treatment and care plan
    /// </summary>
    public class CarePlan
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        public int? EncounterId { get; set; }

        [Required]
        [StringLength(200)]
        public string Title { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Draft, Active, OnHold, Revoked, Completed, EnteredInError, Unknown

        [Required]
        [StringLength(50)]
        public string Intent { get; set; } = "Plan"; // Proposal, Plan, Order, Option

        [StringLength(500)]
        public string? Description { get; set; }

        public string? Goals { get; set; }

        public string? Activities { get; set; }

        public string? Medications { get; set; }

        public string? DietRestrictions { get; set; }

        public string? ExerciseRecommendations { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public Encounter? Encounter { get; set; }
        public ICollection<CarePlanActivity> Activities { get; set; } = new List<CarePlanActivity>();
    }
}