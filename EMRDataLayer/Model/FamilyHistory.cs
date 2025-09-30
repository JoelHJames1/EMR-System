using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR FamilyMemberHistory - Patient's family medical history
    /// </summary>
    public class FamilyHistory
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(50)]
        public string Relationship { get; set; } = string.Empty; // Father, Mother, Sibling, Grandparent, etc.

        [StringLength(50)]
        public string? Gender { get; set; }

        public DateTime? DateOfBirth { get; set; }

        public bool IsDeceased { get; set; } = false;

        public DateTime? DateOfDeath { get; set; }

        [StringLength(200)]
        public string? CauseOfDeath { get; set; }

        public int? AgeAtDeath { get; set; }

        [Required]
        [StringLength(500)]
        public string Condition { get; set; } = string.Empty;

        [StringLength(20)]
        public string? ICDCode { get; set; }

        public int? AgeAtOnset { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation property
        public Patient Patient { get; set; } = null!;
    }
}