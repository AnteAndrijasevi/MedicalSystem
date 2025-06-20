using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.API.DTOs
{
    public class PrescriptionDto
    {
        public int? PrescriptionId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }


        [Required(ErrorMessage = "Medication name is required")]
        [StringLength(200, ErrorMessage = "Medication name cannot be longer than 200 characters")]
        public string MedicationName { get; set; }

        [Required(ErrorMessage = "Dosage is required")]
        [StringLength(100, ErrorMessage = "Dosage cannot be longer than 100 characters")]
        public string Dosage { get; set; }

        public string Instructions { get; set; }

        [Required(ErrorMessage = "Issue date is required")]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }
    }
}