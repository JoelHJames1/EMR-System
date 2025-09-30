using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR ServiceRequest DTO - Referrals to specialists
    /// </summary>
    public class ReferralDto
    {
        public int Id { get; set; }

        [StringLength(50, ErrorMessage = "Referral number cannot exceed 50 characters")]
        public string? ReferralNumber { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Referring provider ID is required")]
        public int ReferringProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Referring provider name cannot exceed 100 characters")]
        public string? ReferringProviderName { get; set; }

        public int? ReferredToProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Referred to provider name cannot exceed 100 characters")]
        public string? ReferredToProviderName { get; set; }

        public int? ReferredToDepartmentId { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Requested";

        [StringLength(50, ErrorMessage = "Priority cannot exceed 50 characters")]
        public string Priority { get; set; } = "Routine";

        [StringLength(200, ErrorMessage = "Specialty cannot exceed 200 characters")]
        public string? Specialty { get; set; }

        [Required(ErrorMessage = "Reason for referral is required")]
        [StringLength(500, ErrorMessage = "Reason for referral cannot exceed 500 characters")]
        public string Reason { get; set; } = string.Empty;

        public string? ClinicalInformation { get; set; }

        [Required(ErrorMessage = "Referral date is required")]
        [DataType(DataType.Date)]
        public DateTime ReferralDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? RequestedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? AppointmentDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CompletedDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ExpirationDate { get; set; }

        [StringLength(500, ErrorMessage = "Instructions cannot exceed 500 characters")]
        public string? Instructions { get; set; }

        [Range(0, 100, ErrorMessage = "Number of visits authorized must be between 0 and 100")]
        public int? NumberOfVisitsAuthorized { get; set; }

        public string? Outcome { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}