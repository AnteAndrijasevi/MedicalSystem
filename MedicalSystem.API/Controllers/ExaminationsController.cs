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
    public class ExaminationsController : ControllerBase
    {
        private readonly IRepository<Examination> _examinationRepository;
        private readonly IRepository<ExaminationType> _examinationTypeRepository;

        public ExaminationsController(IRepositoryFactory repositoryFactory)
        {
            _examinationRepository = repositoryFactory.GetRepository<Examination>();
            _examinationTypeRepository = repositoryFactory.GetRepository<ExaminationType>();
        }

        // GET: api/Examinations
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Examination>>> GetExaminations()
        {
            var examinations = await _examinationRepository.GetAllAsync();
            return Ok(examinations);
        }

        // GET: api/Examinations/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Examination>> GetExamination(int id)
        {
            var examination = await _examinationRepository.GetByIdAsync(id);

            if (examination == null)
            {
                return NotFound();
            }

            return examination;
        }

        // GET: api/Examinations/Patient/5
        [HttpGet("Patient/{patientId}")]
        public async Task<ActionResult<IEnumerable<Examination>>> GetExaminationsByPatient(int patientId)
        {
            var examinations = await _examinationRepository.FindAsync(e => e.PatientId == patientId);

            if (!examinations.Any())
            {
                return NotFound("No examinations found for this patient.");
            }

            return Ok(examinations);
        }

        // GET: api/Examinations/Types
        [HttpGet("Types")]
        public async Task<ActionResult<IEnumerable<ExaminationType>>> GetExaminationTypes()
        {
            var types = await _examinationTypeRepository.GetAllAsync();
            return Ok(types);
        }

        // POST: api/Examinations
        [HttpPost]
        public async Task<ActionResult<Examination>> PostExamination(Examination examination)
        {
            await _examinationRepository.AddAsync(examination);

            return CreatedAtAction(nameof(GetExamination), new { id = examination.ExaminationId }, examination);
        }

        // PUT: api/Examinations/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutExamination(int id, Examination examination)
        {
            if (id != examination.ExaminationId)
            {
                return BadRequest();
            }

            await _examinationRepository.UpdateAsync(examination);

            return NoContent();
        }

        // DELETE: api/Examinations/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteExamination(int id)
        {
            await _examinationRepository.DeleteAsync(id);

            return NoContent();
        }
    }
}