using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MedicalSystem.Core.Models;
using MedicalSystem.Infrastructure.Repositories;
using MedicalSystem.Infrastructure.Services;

namespace MedicalSystem.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MedicalImagesController : ControllerBase
    {
        private readonly IRepository<MedicalImage> _medicalImageRepository;
        private readonly IRepository<Examination> _examinationRepository;
        private readonly IFileService _fileService;

        public MedicalImagesController(
            IRepositoryFactory repositoryFactory,
            IFileService fileService)
        {
            _medicalImageRepository = repositoryFactory.GetRepository<MedicalImage>();
            _examinationRepository = repositoryFactory.GetRepository<Examination>();
            _fileService = fileService;
        }

        // GET: api/MedicalImages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MedicalImage>>> GetMedicalImages()
        {
            var medicalImages = await _medicalImageRepository.GetAllAsync();
            return Ok(medicalImages);
        }

        // GET: api/MedicalImages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<MedicalImage>> GetMedicalImage(int id)
        {
            var medicalImage = await _medicalImageRepository.GetByIdAsync(id);

            if (medicalImage == null)
            {
                return NotFound();
            }

            return medicalImage;
        }

        // GET: api/MedicalImages/File/5
        [HttpGet("File/{id}")]
        public async Task<IActionResult> GetMedicalImageFile(int id)
        {
            var medicalImage = await _medicalImageRepository.GetByIdAsync(id);

            if (medicalImage == null)
            {
                return NotFound();
            }

            try
            {
                var (fileContents, contentType) = await _fileService.GetFileAsync(medicalImage.FilePath);
                return File(fileContents, contentType);
            }
            catch (FileNotFoundException)
            {
                return NotFound("Image file not found.");
            }
        }

        // GET: api/MedicalImages/Examination/5
        [HttpGet("Examination/{examinationId}")]
        public async Task<ActionResult<IEnumerable<MedicalImage>>> GetMedicalImagesByExamination(int examinationId)
        {
            var medicalImages = await _medicalImageRepository.FindAsync(mi => mi.ExaminationId == examinationId);
            return Ok(medicalImages);
        }

        // POST: api/MedicalImages
        [HttpPost]
        public async Task<ActionResult<MedicalImage>> PostMedicalImage([FromForm] IFormFile file, [FromForm] int examinationId)
        {
            // Check if examination exists
            var examination = await _examinationRepository.GetByIdAsync(examinationId);
            if (examination == null)
            {
                return NotFound($"Examination with ID {examinationId} not found.");
            }

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file was uploaded.");
            }

            // Validate file type
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".pdf" };
            var fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
            if (!allowedExtensions.Contains(fileExtension))
            {
                return BadRequest("Invalid file type. Only JPG, PNG, GIF images and PDF documents are allowed.");
            }

            try
            {
                // Save file
                string fileName = await _fileService.SaveFileAsync(file);

                // Create medical image record
                var medicalImage = new MedicalImage
                {
                    ExaminationId = examinationId,
                    FilePath = fileName,
                    UploadDate = DateTime.UtcNow,
                    FileType = file.ContentType
                };

                await _medicalImageRepository.AddAsync(medicalImage);

                return CreatedAtAction(nameof(GetMedicalImage), new { id = medicalImage.ImageId }, medicalImage);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error uploading file: {ex.Message}");
            }
        }

        // DELETE: api/MedicalImages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMedicalImage(int id)
        {
            var medicalImage = await _medicalImageRepository.GetByIdAsync(id);
            if (medicalImage == null)
            {
                return NotFound();
            }

            try
            {
                // Delete file
                await _fileService.DeleteFileAsync(medicalImage.FilePath);

                // Delete record
                await _medicalImageRepository.DeleteAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, $"Error deleting image: {ex.Message}");
            }
        }
    }
}