using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.API.DTOs
{
    public class PatientDto
    {
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "First name is required")]
        [StringLength(100, ErrorMessage = "First name cannot be longer than 100 characters")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required")]
        [StringLength(100, ErrorMessage = "Last name cannot be longer than 100 characters")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "OIB is required")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB must be exactly 11 characters")]
        [RegularExpression(@"^[0-9]{11}$", ErrorMessage = "OIB must contain only digits")]
        public string OIB { get; set; }

        [Required(ErrorMessage = "Date of birth is required")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Gender is required")]
        [RegularExpression(@"^[MFO]$", ErrorMessage = "Gender must be 'M', 'F', or 'O'")]
        public string Gender { get; set; }

        [StringLength(20, ErrorMessage = "Patient number cannot be longer than 20 characters")]
        public string PatientNumber { get; set; }
    }
}