using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Infrastructure.Repositories;
using System.Linq;

namespace MedicalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PrescriptionsController : ControllerBase
    {
        private readonly IRepository<Prescription> _prescriptionRepository;

        public PrescriptionsController(IRepositoryFactory repositoryFactory)
        {
            _prescriptionRepository = repositoryFactory.GetRepository<Prescription>();
        }

        // GET: api/Prescriptions
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptions()
        {
            var prescriptions = await _prescriptionRepository.GetAllAsync();
            return Ok(prescriptions);
        }

        // GET: api/Prescriptions/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Prescription>> GetPrescription(int id)
        {
            var prescription = await _prescriptionRepository.GetByIdAsync(id);

            if (prescription == null)
            {
                return NotFound();
            }

            return prescription;
        }

        // GET: api/Prescriptions/Patient/5
        [HttpGet("Patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Prescription>>> GetPrescriptionsByPatient(int patientId)
        {
            var prescriptions = await _prescriptionRepository.FindAsync(p => p.PatientId == patientId);

            if (!prescriptions.Any())
            {
                return NotFound("No prescriptions found for this patient.");
            }

            return Ok(prescriptions);
        }

        // POST: api/Prescriptions
        [HttpPost]
        public async Task<ActionResult<Prescription>> PostPrescription(Prescription prescription)
        {
            await _prescriptionRepository.AddAsync(prescription);

            return CreatedAtAction(nameof(GetPrescription), new { id = prescription.PrescriptionId }, prescription);
        }

        // PUT: api/Prescriptions/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPrescription(int id, Prescription prescription)
        {
            if (id != prescription.PrescriptionId)
            {
                return BadRequest();
            }

            await _prescriptionRepository.UpdateAsync(prescription);

            return NoContent();
        }

        // DELETE: api/Prescriptions/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrescription(int id)
        {
            await _prescriptionRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}