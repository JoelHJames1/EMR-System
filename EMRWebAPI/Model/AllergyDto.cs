using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class AllergyDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Allergen is required")]
        [StringLength(200, ErrorMessage = "Allergen cannot exceed 200 characters")]
        public string Allergen { get; set; } = string.Empty;

        [Required(ErrorMessage = "Allergy type is required")]
        [StringLength(100, ErrorMessage = "Allergy type cannot exceed 100 characters")]
        public string AllergyType { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Severity cannot exceed 50 characters")]
        public string Severity { get; set; } = "Moderate";

        [StringLength(500, ErrorMessage = "Reaction cannot exceed 500 characters")]
        public string? Reaction { get; set; }

        [DataType(DataType.Date)]
        public DateTime? OnsetDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? VerifiedDate { get; set; }

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Active";

        [StringLength(500, ErrorMessage = "Notes cannot exceed 500 characters")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}