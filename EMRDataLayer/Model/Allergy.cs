using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Allergy
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        [StringLength(200)]
        public string Allergen { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string AllergyType { get; set; } = string.Empty; // Drug, Food, Environmental, etc.

        [StringLength(50)]
        public string Severity { get; set; } = "Moderate"; // Mild, Moderate, Severe, Life-threatening

        [StringLength(500)]
        public string? Reaction { get; set; }

        public DateTime? OnsetDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

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