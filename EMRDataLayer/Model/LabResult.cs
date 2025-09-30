using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class LabResult
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int LabOrderId { get; set; }

        [Required]
        [StringLength(200)]
        public string TestComponent { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Value { get; set; }

        [StringLength(50)]
        public string? Unit { get; set; }

        [StringLength(100)]
        public string? ReferenceRange { get; set; }

        [StringLength(50)]
        public string? Flag { get; set; } // Normal, High, Low, Critical

        public DateTime? ResultDate { get; set; }

        public string? Comments { get; set; }

        [StringLength(100)]
        public string? PerformedBy { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public LabOrder LabOrder { get; set; } = null!;
    }
}