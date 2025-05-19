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

        public PatientsController(RepositoryFactory repositoryFactory)
        {
            _patientRepository = repositoryFactory.GetPatientRepository();
        }

        // GET: api/Patients
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Patient>>> GetPatients()
        {
            var patients = await _patientRepository.GetAllAsync();
            return Ok(patients);
        }

        // GET: api/Patients/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Patient>> GetPatient(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);

            if (patient == null)
            {
                return NotFound();
            }

            return patient;
        }

        // GET: api/Patients/search?term=searchTerm
        [HttpGet("search")]
        public async Task<ActionResult<IEnumerable<Patient>>> SearchPatients([FromQuery] string term)
        {
            var patients = await _patientRepository.SearchByNameOrOibAsync(term);
            return Ok(patients);
        }

        // GET: api/Patients/5/export
        [HttpGet("{id}/export")]
        public async Task<IActionResult> ExportPatient(int id)
        {
            var csvData = await _patientRepository.ExportToCsvAsync(id);

            if (csvData == null)
            {
                return NotFound();
            }

            return File(csvData, "text/csv", $"patient_{id}_data.csv");
        }

        // POST: api/Patients
        // POST: api/Patients
        [HttpPost]
        public async Task<ActionResult<Patient>> PostPatient(PatientDto patientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var patient = new Patient
            {
                FirstName = patientDto.FirstName,
                LastName = patientDto.LastName,
                OIB = patientDto.OIB,
                DateOfBirth = patientDto.DateOfBirth,
                Gender = patientDto.Gender,
                // Generate patient number if not provided
                PatientNumber = patientDto.PatientNumber ?? $"P-{patientDto.DateOfBirth.Year}-{DateTime.Now.Ticks % 10000}"
            };

            try
            {
                await _patientRepository.AddAsync(patient);
                return CreatedAtAction(nameof(GetPatient), new { id = patient.PatientId }, patient);
            }
            catch (DbUpdateException ex)
            {
                // Check if this is a unique constraint violation
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                {
                    return Conflict("A patient with this OIB already exists.");
                }
                throw;
            }
        }

        // PUT: api/Patients/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPatient(int id, PatientDto patientDto)
        {
            if (patientDto.PatientId != id)
            {
                return BadRequest("Patient ID in the URL does not match the ID in the request body.");
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

            patient.FirstName = patientDto.FirstName;
            patient.LastName = patientDto.LastName;
            patient.OIB = patientDto.OIB;
            patient.DateOfBirth = patientDto.DateOfBirth;
            patient.Gender = patientDto.Gender;

            // Update patient number if provided
            if (!string.IsNullOrEmpty(patientDto.PatientNumber))
            {
                patient.PatientNumber = patientDto.PatientNumber;
            }

            try
            {
                await _patientRepository.UpdateAsync(patient);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                // Check if this is a unique constraint violation
                if (ex.InnerException != null && ex.InnerException.Message.Contains("duplicate key"))
                {
                    return Conflict("A patient with this OIB already exists.");
                }
                throw;
            }
        }

       

        // DELETE: api/Patients/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await _patientRepository.GetByIdAsync(id);
            if (patient == null)
            {
                return NotFound();
            }

            await _patientRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}