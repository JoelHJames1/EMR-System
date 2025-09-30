using System;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    public class Appointment
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int PatientId { get; set; }

        [Required]
        public int ProviderId { get; set; }

        [Required]
        public DateTime AppointmentDate { get; set; }

        [Required]
        public TimeSpan StartTime { get; set; }

        [Required]
        public TimeSpan EndTime { get; set; }

        [Required]
        [StringLength(50)]
        public string Status { get; set; } = "Scheduled"; // Scheduled, Confirmed, InProgress, Completed, Cancelled, NoShow

        [Required]
        [StringLength(100)]
        public string AppointmentType { get; set; } = string.Empty; // Consultation, Follow-up, Emergency, etc.

        [StringLength(500)]
        public string? ReasonForVisit { get; set; }

        [StringLength(1000)]
        public string? Notes { get; set; }

        [StringLength(100)]
        public string? RoomNumber { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        [StringLength(36)]
        public string? CreatedBy { get; set; }

        [StringLength(36)]
        public string? ModifiedBy { get; set; }

        // Navigation properties
        public Patient Patient { get; set; } = null!;
        public Provider Provider { get; set; } = null!;
    }
}