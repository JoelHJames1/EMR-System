using System;
using System.ComponentModel.DataAnnotations;

namespace EMRWebAPI.Model
{
    public class PatientDto
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(50, ErrorMessage = "First name cannot exceed 50 characters")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(50, ErrorMessage = "Last name cannot exceed 50 characters")]
        public string LastName { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "Middle name cannot exceed 50 characters")]
        public string? MiddleName { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [StringLength(10, ErrorMessage = "Gender cannot exceed 10 characters")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Social security number cannot exceed 20 characters")]
        public string? SocialSecurityNumber { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [StringLength(100, ErrorMessage = "Email cannot exceed 100 characters")]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Phone number is required")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid phone number format")]
        public string PhoneNumber { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Emergency contact cannot exceed 20 characters")]
        [Phone(ErrorMessage = "Invalid emergency contact phone number format")]
        public string? EmergencyContact { get; set; }

        [StringLength(100, ErrorMessage = "Emergency contact name cannot exceed 100 characters")]
        public string? EmergencyContactName { get; set; }

        [StringLength(50, ErrorMessage = "Blood type cannot exceed 50 characters")]
        public string? BloodType { get; set; }

        [StringLength(20, ErrorMessage = "Marital status cannot exceed 20 characters")]
        public string? MaritalStatus { get; set; }

        [StringLength(100, ErrorMessage = "Occupation cannot exceed 100 characters")]
        public string? Occupation { get; set; }

        [StringLength(100, ErrorMessage = "Employer cannot exceed 100 characters")]
        public string? Employer { get; set; }

        [StringLength(50, ErrorMessage = "Preferred language cannot exceed 50 characters")]
        public string? PreferredLanguage { get; set; }

        [StringLength(100, ErrorMessage = "Ethnicity cannot exceed 100 characters")]
        public string? Ethnicity { get; set; }

        [StringLength(100, ErrorMessage = "Race cannot exceed 100 characters")]
        public string? Race { get; set; }

        public bool IsActive { get; set; } = true;

        public AddressDto? Address { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? ModifiedDate { get; set; }
    }
}