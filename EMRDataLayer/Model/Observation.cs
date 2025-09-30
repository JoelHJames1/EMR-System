using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR Observation - Clinical measurements and findings
    /// </summary>
    public class Observation
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        public int? EncounterId { get; set; }

        public int? ProviderId { get; set; }

        [Required]
        [StringLength(100)]
        public string ObservationType { get; set; } = string.Empty; // VitalSign, Laboratory, Imaging, Social, Physical

        [Required]
        [StringLength(200)]
        public string ObservationCode { get; set; } = string.Empty; // LOINC code

        [StringLength(500)]
        public string ObservationName { get; set; } = string.Empty;

        [StringLength(50)]
        public string Status { get; set; } = "Final"; // Registered, Preliminary, Final, Amended, Corrected, Cancelled

        [StringLength(500)]
        public string? ValueString { get; set; }

        [Column(TypeName = "decimal(18,4)")]
        public decimal? ValueNumeric { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(200)]
        public string? ReferenceRange { get; set; }

        [StringLength(50)]
        public string? Interpretation { get; set; } // Normal, High, Low, Critical, Abnormal

        public DateTime ObservationDateTime { get; set; } = DateTime.UtcNow;

        public DateTime? IssuedDateTime { get; set; }

        [StringLength(50)]
        public string? Method { get; set; }

        [StringLength(100)]
        public string? BodySite { get; set; }

        [StringLength(100)]
        public string? Device { get; set; }

        public string? Notes { get; set; }

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