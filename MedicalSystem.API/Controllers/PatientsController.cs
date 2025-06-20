using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.API.DTOs;
using System.Linq;
using Microsoft.EntityFrameworkCore;

namespace MedicalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientRepository _patientRepository;

        public PatientsController(IRepositoryFactory repositoryFactory)
        {
            _patientRepository = repositoryFactory.GetPatientRepository();
        }

        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(PatientDto patientDto)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                // PROVJERA POSTOJANJA OIB-a
                var existingPatients = await _patientRepository.FindAsync(p => p.OIB == patientDto.OIB);
                if (existingPatients.Any())
                {
                    return Conflict("Pacijent s ovim OIB-om već postoji.");
                }

                // KREIRANJE PATIENT ENTITY-ja
                var patient = new Patient
                {
                    FirstName = patientDto.FirstName?.Trim(),
                    LastName = patientDto.LastName?.Trim(),
                    OIB = patientDto.OIB?.Trim(),
                    DateOfBirth = patientDto.DateOfBirth,
                    Gender = patientDto.Gender?.Trim(),

                    // **OVDJE JE LOGIKA ZA PatientNumber:**
                    PatientNumber = !string.IsNullOrWhiteSpace(patientDto.PatientNumber)
                        ? patientDto.PatientNumber.Trim()
                        : await GenerateUniquePatientNumberAsync(patientDto.DateOfBirth),

                    ContactNumber = string.IsNullOrWhiteSpace(patientDto.ContactNumber)
                        ? ""
                        : patientDto.ContactNumber.Trim()
                };

                var createdPatient = await _patientRepository.AddAsync(patient);
                return CreatedAtAction(nameof(GetPatient), new { id = createdPatient.PatientId }, createdPatient);
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                {
                    return Conflict("Pacijent s ovim OIB-om već postoji.");
                }
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // **HELPER METODA za generiranje jedinstvenog PatientNumber-a**
        private async Task<string> GenerateUniquePatientNumberAsync(DateTime dateOfBirth)
        {
            var year = dateOfBirth.Year;
            var baseNumber = $"P-{year}-";

            // Pronađi najveći postojeći broj za tu godinu
            var existingNumbers = await _patientRepository.FindAsync(p => p.PatientNumber.StartsWith(baseNumber));

            if (!existingNumbers.Any())
            {
                return $"{baseNumber}001"; // Prvi pacijent te godine
            }

            var maxNumber = existingNumbers
                .Select(p => p.PatientNumber)
                .Where(pn => pn.Length > baseNumber.Length)
                .Select(pn =>
                {
                    var suffix = pn.Substring(baseNumber.Length);
                    return int.TryParse(suffix, out var num) ? num : 0;
                })
                .DefaultIfEmpty(0)
                .Max();

            return $"{baseNumber}{(maxNumber + 1):D3}";
        }

        // **ALTERNATIVNA JEDNOSTAVNA METODA**
        private string GenerateSimplePatientNumber(DateTime dateOfBirth)
        {
            var year = dateOfBirth.Year;
            var random = new Random().Next(1, 9999);
            return $"P-{year}-{random:D4}";
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            try
            {
                var patients = await _patientRepository.GetAllAsync();
                return Ok(patients);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);

                if (patient == null)
                {
                    return NotFound();
                }

                return patient;
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Patients/search?term=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string term)
        {
            try
            {
                if (string.IsNullOrEmpty(term))
                {
                    var allPatients = await _patientRepository.GetAllAsync();
                    return Ok(allPatients);
                }

                var patients = await _patientRepository.SearchByNameOrOibAsync(term);
                return Ok(patients);
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDto patientDto)
        {
            try
            {
                if (patientDto.PatientId != id)
                {
                    return BadRequest("Patient ID mismatch.");
                }

                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }

                // Ažuriranje svojstava (PatientNumber se NE mijenja)
                patient.FirstName = patientDto.FirstName?.Trim();
                patient.LastName = patientDto.LastName?.Trim();
                patient.OIB = patientDto.OIB?.Trim();
                patient.DateOfBirth = patientDto.DateOfBirth;
                patient.Gender = patientDto.Gender?.Trim();
                patient.ContactNumber = string.IsNullOrWhiteSpace(patientDto.ContactNumber)
                    ? ""
                    : patientDto.ContactNumber.Trim();

                // PatientNumber se NE mijenja kod UPDATE-a!
                // patient.PatientNumber ostaje isti

                await _patientRepository.UpdateAsync(patient);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                {
                    return Conflict("Pacijent s ovim OIB-om već postoji.");
                }
                return StatusCode(500, $"Database error: {ex.Message}");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            try
            {
                var patient = await _patientRepository.GetByIdAsync(id);
                if (patient == null)
                {
                    return NotFound();
                }

                await _patientRepository.DeleteAsync(id);
                return NoContent();
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }

        // GET: api/Patients/5/export
        [HttpGet("{id}/export")]
        public async Task<IActionResult> ExportPatient(int id)
        {
            try
            {
                var csvData = await _patientRepository.ExportToCsvAsync(id);

                if (csvData == null)
                {
                    return NotFound();
                }

                return File(csvData, "text/csv", $"patient_{id}_data.csv");
            }
            catch (System.Exception ex)
            {
                return StatusCode(500, $"Internal server error: {ex.Message}");
            }
        }
    }
}