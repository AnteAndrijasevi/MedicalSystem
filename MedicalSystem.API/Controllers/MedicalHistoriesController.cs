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
    public class MedicalHistoriesController : ControllerBase
    {
        private readonly IRepository<MedicalHistory> _medicalHistoryRepository;

        public MedicalHistoriesController(IRepositoryFactory repositoryFactory)
        {
            _medicalHistoryRepository = repositoryFactory.GetRepository<MedicalHistory>();
        }

        // GET: api/MedicalHistories
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalHistory>>> GetMedicalHistories()
        {
            var medicalHistories = await _medicalHistoryRepository.GetAllAsync();
            return Ok(medicalHistories);
        }

        // GET: api/MedicalHistories/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalHistory>> GetMedicalHistory(int id)
        {
            var medicalHistory = await _medicalHistoryRepository.GetByIdAsync(id);

            if (medicalHistory == null)
            {
                return NotFound();
            }

            return medicalHistory;
        }

        // GET: api/MedicalHistories/Patient/5
        [HttpGet("Patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<MedicalHistory>>> GetMedicalHistoriesByPatient(int patientId)
        {
            var medicalHistories = await _medicalHistoryRepository.FindAsync(mh => mh.PatientId == patientId);

            if (!medicalHistories.Any())
            {
                return NotFound("No medical histories found for this patient.");
            }

            return Ok(medicalHistories);
        }

        // POST: api/MedicalHistories
        [HttpPost]
        public async Task<ActionResult<MedicalHistory>> PostMedicalHistory(MedicalHistory medicalHistory)
        {
            await _medicalHistoryRepository.AddAsync(medicalHistory);

            return CreatedAtAction(nameof(GetMedicalHistory), new { id = medicalHistory.MedicalHistoryId }, medicalHistory);
        }

        // PUT: api/MedicalHistories/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMedicalHistory(int id, MedicalHistory medicalHistory)
        {
            if (id != medicalHistory.MedicalHistoryId)
            {
                return BadRequest();
            }

            await _medicalHistoryRepository.UpdateAsync(medicalHistory);

            return NoContent();
        }

        // DELETE: api/MedicalHistories/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalHistory(int id)
        {
            await _medicalHistoryRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}