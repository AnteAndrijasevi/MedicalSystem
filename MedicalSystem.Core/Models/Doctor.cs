using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Core.Models
{
    public class Doctor
    {
        public Doctor()
        {
            Examinations = new HashSet<Examination>();
            Prescriptions = new HashSet<Prescription>();
        }

        [Key]
        public int DoctorId { get; set; }

        [Required]
        [StringLength(100)]
        public string FirstName { get; set; }

        [Required]
        [StringLength(100)]
        public string LastName { get; set; }

        [StringLength(100)]
        public string Specialization { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        // Navigation properties
        public virtual ICollection<Examination> Examinations { get; set; }
        public virtual ICollection<Prescription> Prescriptions { get; set; }
    }
}