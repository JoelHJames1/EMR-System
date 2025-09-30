using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    /// <summary>
    /// Hospital/Clinic Department or Service DTO
    /// </summary>
    public class DepartmentDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Department name is required")]
        [StringLength(100, ErrorMessage = "Department name cannot exceed 100 characters")]
        public string DepartmentName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Department code is required")]
        [StringLength(50, ErrorMessage = "Department code cannot exceed 50 characters")]
        public string DepartmentCode { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Specialty cannot exceed 100 characters")]
        public string? Specialty { get; set; }

        [StringLength(500, ErrorMessage = "Description cannot exceed 500 characters")]
        public string? Description { get; set; }

        [StringLength(100, ErrorMessage = "Head of department cannot exceed 100 characters")]
        public string? HeadOfDepartment { get; set; }

        public int? ManagerId { get; set; }

        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string? Phone { get; set; }

        [StringLength(20, ErrorMessage = "Fax number cannot exceed 20 characters")]
        public string? FaxNumber { get; set; }

        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string? Email { get; set; }

        public string? OperatingHours { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}