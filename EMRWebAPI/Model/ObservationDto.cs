using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR Observation DTO - Clinical measurements and findings
    /// </summary>
    public class ObservationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        public int? ProviderId { get; set; }

        [Required(ErrorMessage = "Observation type is required")]
        [StringLength(100, ErrorMessage = "Observation type cannot exceed 100 characters")]
        public string ObservationType { get; set; } = string.Empty;

        [Required(ErrorMessage = "Observation code is required")]
        [StringLength(200, ErrorMessage = "Observation code cannot exceed 200 characters")]
        public string ObservationCode { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Observation name cannot exceed 500 characters")]
        public string? ObservationName { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Final";

        [StringLength(500, ErrorMessage = "Value string cannot exceed 500 characters")]
        public string? ValueString { get; set; }

        public decimal? ValueNumeric { get; set; }

        [StringLength(50, ErrorMessage = "Unit cannot exceed 50 characters")]
        public string? Unit { get; set; }

        [StringLength(200, ErrorMessage = "Reference range cannot exceed 200 characters")]
        public string? ReferenceRange { get; set; }

        [StringLength(50, ErrorMessage = "Interpretation cannot exceed 50 characters")]
        public string? Interpretation { get; set; }

        [Required(ErrorMessage = "Observation date time is required")]
        [DataType(DataType.DateTime)]
        public DateTime ObservationDateTime { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? IssuedDateTime { get; set; }

        [StringLength(50, ErrorMessage = "Method cannot exceed 50 characters")]
        public string? Method { get; set; }

        [StringLength(100, ErrorMessage = "Body site cannot exceed 100 characters")]
        public string? BodySite { get; set; }

        [StringLength(100, ErrorMessage = "Device cannot exceed 100 characters")]
        public string? Device { get; set; }

        public string? Notes { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}