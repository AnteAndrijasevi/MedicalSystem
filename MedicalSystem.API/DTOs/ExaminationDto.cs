using System;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.API.DTOs
{
    public class ExaminationDto
    {
        public int? ExaminationId { get; set; }

        [Required(ErrorMessage = "Patient ID is required")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Examination type is required")]
        public int TypeId { get; set; }

        public int? DoctorId { get; set; }

        [Required(ErrorMessage = "Examination date is required")]
        [DataType(DataType.Date)]
        public DateTime ExaminationDate { get; set; }

        [Required(ErrorMessage = "Examination time is required")]
        [DataType(DataType.Time)]
        public TimeSpan ExaminationTime { get; set; }

        public string Notes { get; set; }
    }
}