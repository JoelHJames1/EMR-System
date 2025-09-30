using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class LabOrder
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        [StringLength(200)]
        public string TestName { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string TestCode { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; } // Blood, Urine, Imaging, etc.

        [StringLength(50)]
        public string Priority { get; set; } = "Routine"; // Stat, Urgent, Routine

        [StringLength(50)]
        public string Status { get; set; } = "Ordered"; // Ordered, InProgress, Completed, Cancelled

        public DateTime OrderedDate { get; set; } = DateTime.UtcNow;

        public DateTime? CollectedDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string? ClinicalNotes { get; set; }

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
        public ICollection<LabResult> LabResults { get; set; } = new List<LabResult>();
    }
}