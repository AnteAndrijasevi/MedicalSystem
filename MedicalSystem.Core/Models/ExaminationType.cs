using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Core.Models
{
    public class ExaminationType
    {
        public ExaminationType()
        {
            Examinations = new HashSet<Examination>();
        }

        [Key]
        public int TypeId { get; set; }

        [Required]
        [StringLength(10)]
        public string Code { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        public string? Description { get; set; }

        // Navigation property
        public virtual ICollection<Examination> Examinations { get; set; }
    }
}