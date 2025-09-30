using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace EMRDataLayer.Model
{
    /// <summary>
    /// HL7 FHIR Location - Physical places where services are provided
    /// </summary>
    public class Location
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string LocationName { get; set; } = string.Empty;

        [StringLength(50)]
        public string? LocationCode { get; set; }

        [Required]
        [StringLength(50)]
        public string LocationType { get; set; } = string.Empty; // Room, Ward, Building, Floor, Area, Vehicle

        [StringLength(50)]
        public string Status { get; set; } = "Active"; // Active, Suspended, Inactive

        [StringLength(500)]
        public string? Description { get; set; }

        [StringLength(50)]
        public string? RoomNumber { get; set; }

        [StringLength(50)]
        public string? Floor { get; set; }

        [StringLength(100)]
        public string? Building { get; set; }

        public int? DepartmentId { get; set; }

        public int? AddressId { get; set; }

        [StringLength(20)]
        [Phone]
        public string? PhoneNumber { get; set; }

        [StringLength(20)]
        [Phone]
        public string? FaxNumber { get; set; }

        public int? Capacity { get; set; }

        public string? OperatingHours { get; set; }

        public string? AvailabilityExceptions { get; set; }

        [StringLength(100)]
        public string? ManagingOrganization { get; set; }

        public bool IsAvailable { get; set; } = true;

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

        public DateTime? ModifiedDate { get; set; }

        // Navigation properties
        public Department? Department { get; set; }
        public Address? Address { get; set; }
        public ICollection<Encounter> Encounters { get; set; } = new List<Encounter>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
        public ICollection<Procedure> Procedures { get; set; } = new List<Procedure>();
    }
}