using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Medication
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string GenericName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? BrandName { get; set; }

        [Required]
        [StringLength(100)]
        public string Category { get; set; } = string.Empty; // Antibiotic, Analgesic, etc.

        [StringLength(50)]
        public string? Form { get; set; } // Tablet, Capsule, Liquid, etc.

        [StringLength(100)]
        public string? Strength { get; set; }

        [StringLength(20)]
        public string? NDC { get; set; } // National Drug Code

        public string? Description { get; set; }

        public string? SideEffects { get; set; }

        public string? Contraindications { get; set; }

        public bool IsControlledSubstance { get; set; } = false;

        [StringLength(20)]
        public string? DEASchedule { get; set; } // Schedule II, III, IV, V

        public bool IsActive { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public ICollection<Prescription> Prescriptions { get; set; } = new List<Prescription>();
    }
}