using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// Hospital/Clinic Department or Service
    /// </summary>
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? DepartmentCode { get; set; }

        [StringLength(100)]
        public string? Specialty { get; set; } // Cardiology, Pediatrics, Surgery, etc.

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        [Phone]
        public string? FaxNumber { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string? Email { get; set; }

        public int? ManagerId { get; set; } // User/Provider who manages department

        public bool IsActive { get; set; } = true;

        public string? OperatingHours { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public ICollection<Location> Locations { get; set; } = new List<Location>();
        public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
    }
}