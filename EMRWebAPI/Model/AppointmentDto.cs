using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class AppointmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [StringLength(100, ErrorMessage = "Patient name cannot exceed 100 characters")]
        public string? PatientName { get; set; }

        [Required(ErrorMessage = "Provider ID is required")]
        public int ProviderId { get; set; }

        [StringLength(100, ErrorMessage = "Provider name cannot exceed 100 characters")]
        public string? ProviderName { get; set; }

        [Required(ErrorMessage = "Appointment date is required")]
        [DataType(DataType.Date)]
        public DateTime AppointmentDate { get; set; }

        [Required(ErrorMessage = "Start time is required")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End time is required")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Scheduled";

        [Required(ErrorMessage = "Appointment type is required")]
        [StringLength(100, ErrorMessage = "Appointment type cannot exceed 100 characters")]
        public string AppointmentType { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Reason for visit cannot exceed 500 characters")]
        public string? ReasonForVisit { get; set; }

        [StringLength(1000, ErrorMessage = "Notes cannot exceed 1000 characters")]
        public string? Notes { get; set; }

        [StringLength(100, ErrorMessage = "Room number cannot exceed 100 characters")]
        public string? RoomNumber { get; set; }

        public int? LocationId { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}