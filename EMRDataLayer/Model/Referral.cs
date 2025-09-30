using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR ServiceRequest - Referrals to specialists
    /// </summary>
    public class Referral
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ReferralNumber { get; set; } = string.Empty;

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ReferringProviderId { get; set; }

        public int? ReferredToProviderId { get; set; }

        public int? ReferredToDepartmentId { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Requested"; // Draft, Active, OnHold, Revoked, Completed, EnteredInError, Unknown

        [StringLength(50)]
        public string Priority { get; set; } = "Routine"; // Routine, Urgent, ASAP, Stat

        [StringLength(200)]
        public string? Specialty { get; set; }

        [StringLength(500)]
        public string ReasonForReferral { get; set; } = string.Empty;

        public string? ClinicalInformation { get; set; }

        public DateTime ReferralDate { get; set; } = DateTime.UtcNow;

        public DateTime? RequestedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [StringLength(500)]
        public string? Instructions { get; set; }

        public int? NumberOfVisitsAuthorized { get; set; }

        public string? Outcome { get; set; }

        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider ReferringProvider { get; set; } = null!;
        public Provider? ReferredToProvider { get; set; }
    }
}