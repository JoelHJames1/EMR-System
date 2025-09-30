using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Immunization
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string VaccineName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? CVXCode { get; set; } // Vaccine Administered CVX Code

        [Required]
        public DateTime AdministeredDate { get; set; }

        [StringLength(100)]
        public string? AdministeredBy { get; set; }

        [StringLength(100)]
        public string? Route { get; set; } // IM, Oral, Subcutaneous, etc.

        [StringLength(100)]
        public string? Site { get; set; } // Left arm, Right arm, etc.

        [StringLength(100)]
        public string? LotNumber { get; set; }

        [StringLength(200)]
        public string? Manufacturer { get; set; }

        public DateTime? ExpirationDate { get; set; }

        [StringLength(50)]
        public string? DoseNumber { get; set; }

        [StringLength(500)]
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