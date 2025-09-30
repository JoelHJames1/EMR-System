using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// HL7 FHIR Location DTO - Physical places where services are provided
    /// </summary>
    public class LocationDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Location name is required")]
        [StringLength(100, ErrorMessage = "Location name cannot exceed 100 characters")]
        public string LocationName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Location code cannot exceed 50 characters")]
        public string? LocationCode { get; set; }

        [Required(ErrorMessage = "Location type is required")]
        [StringLength(50, ErrorMessage = "Location type cannot exceed 50 characters")]
        public string LocationType { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Status cannot exceed 50 characters")]
        public string Status { get; set; } = "Active";

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(50, ErrorMessage = "Room number cannot exceed 50 characters")]
        public string? RoomNumber { get; set; }

        [StringLength(50, ErrorMessage = "Floor cannot exceed 50 characters")]
        public string? Floor { get; set; }

        [StringLength(100, ErrorMessage = "Building cannot exceed 100 characters")]
        public string? Building { get; set; }

        public int? DepartmentId { get; set; }

        public AddressDto? Address { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [StringLength(20, ErrorMessage = "Fax number cannot exceed 20 characters")]
        public string? FaxNumber { get; set; }

        [Range(0, 10000, ErrorMessage = "Capacity must be between 0 and 10000")]
        public int? Capacity { get; set; }

        public string? OperatingHours { get; set; }

        public string? AvailabilityExceptions { get; set; }

        [StringLength(100, ErrorMessage = "Managing organization cannot exceed 100 characters")]
        public string? ManagingOrganization { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsAvailable { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}