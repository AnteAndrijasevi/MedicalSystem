using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.API.DTOs
{
    public class MedicalHistoryDto
    {
        public int? MedicalHistoryId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Disease name is required")]
        [StringLength(200, ErrorMessage = "Disease name cannot be longer than 200 characters")]
        public string DiseaseName { get; set; }

        [Required(ErrorMessage = "Start date is required")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }
    }
}