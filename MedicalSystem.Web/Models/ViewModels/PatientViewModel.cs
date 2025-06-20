using System.ComponentModel.DataAnnotations;

namespace MedicalSystem.Web.Models.ViewModels
{
    public class PatientViewModel
    {
        public int? PatientId { get; set; }

        [Required(ErrorMessage = "Ime je obavezno")]
        [Display(Name = "Ime")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Prezime je obavezno")]
        [Display(Name = "Prezime")]
        public string LastName { get; set; }

        [Required(ErrorMessage = "OIB je obavezan")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "OIB mora imati točno 11 znamenki")]
        [Display(Name = "OIB")]
        public string OIB { get; set; }

        [Required(ErrorMessage = "Datum rođenja je obavezan")]
        [DataType(DataType.Date)]
        [Display(Name = "Datum rođenja")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Spol je obavezan")]
        [Display(Name = "Spol")]
        public string Gender { get; set; }

        [Display(Name = "Broj pacijenta")]
        public string PatientNumber { get; set; }

        [Display(Name = "Kontakt broj")]
        public string ContactNumber { get; set; }

        // Computed properties
        public string FullName => $"{FirstName} {LastName}";
        public int Age => DateTime.Now.Year - DateOfBirth.Year;
        public string GenderText => Gender switch
        {
            "M" => "Muški",
            "F" => "Ženski",
            "O" => "Ostalo",
            _ => Gender
        };
    }
}