using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class MedicationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Medication name is required")]
        [StringLength(200, ErrorMessage = "Medication name cannot exceed 200 characters")]
        public string MedicationName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Generic name is required")]
        [StringLength(200, ErrorMessage = "Generic name cannot exceed 200 characters")]
        public string GenericName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Brand name cannot exceed 100 characters")]
        public string? BrandName { get; set; }

        [Required(ErrorMessage = "Category is required")]
        [StringLength(100, ErrorMessage = "Category cannot exceed 100 characters")]
        public string Category { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Form cannot exceed 50 characters")]
        public string? Form { get; set; }

        [StringLength(100, ErrorMessage = "Strength cannot exceed 100 characters")]
        public string? Strength { get; set; }

        [StringLength(100, ErrorMessage = "Route cannot exceed 100 characters")]
        public string? Route { get; set; }

        [Required(ErrorMessage = "NDC is required")]
        [StringLength(20, ErrorMessage = "NDC cannot exceed 20 characters")]
        public string NDC { get; set; } = string.Empty;

        public string? Description { get; set; }

        public string? SideEffects { get; set; }

        public string? Contraindications { get; set; }

        public bool IsControlled { get; set; } = false;

        [StringLength(20, ErrorMessage = "DEA schedule cannot exceed 20 characters")]
        public string? DEASchedule { get; set; }

        [StringLength(200, ErrorMessage = "Manufacturer cannot exceed 200 characters")]
        public string? Manufacturer { get; set; }

        public string? Instructions { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}