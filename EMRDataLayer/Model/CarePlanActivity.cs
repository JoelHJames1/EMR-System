using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// Individual activities within a care plan
    /// </summary>
    public class CarePlanActivity
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int CarePlanId { get; set; }

        [Required]
        [StringLength(200)]
        public string ActivityName { get; set; } = string.Empty;

        [StringLength(100)]
        public string? Category { get; set; } // Medication, Diet, Procedure, Appointment, etc.

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string Status { get; set; } = "NotStarted"; // NotStarted, Scheduled, InProgress, OnHold, Completed, Cancelled

        public DateTime? ScheduledDate { get; set; }

        public DateTime? CompletedDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        // Navigation property
        public CarePlan CarePlan { get; set; } = null!;
    }
}