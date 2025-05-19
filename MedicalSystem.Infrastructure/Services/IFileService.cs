using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;

namespace MedicalSystem.Infrastructure.Services
{
    public interface IFileService
    {
        Task<string> SaveFileAsync(IFormFile file);
        Task<(byte[] FileContents, string ContentType)> GetFileAsync(string fileName);
        Task DeleteFileAsync(string fileName);
    }

    public class LocalFileService : IFileService
    {
        private readonly string _uploadDirectory;

        public LocalFileService(IConfiguration configuration)
        {
            _uploadDirectory = configuration["FileStorage:UploadDirectory"] ?? "Uploads";

            // Ensure directory exists
            if (!Directory.Exists(_uploadDirectory))
            {
                Directory.CreateDirectory(_uploadDirectory);
            }
        }

        public async Task<string> SaveFileAsync(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                throw new ArgumentException("File is empty or null", nameof(file));
            }

            // Create a unique file name
            string uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(_uploadDirectory, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return uniqueFileName;
        }

        public async Task<(byte[] FileContents, string ContentType)> GetFileAsync(string fileName)
        {
            string filePath = Path.Combine(_uploadDirectory, fileName);

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found", fileName);
            }

            byte[] fileContents = await File.ReadAllBytesAsync(filePath);
            string contentType = GetContentType(Path.GetExtension(fileName));

            return (fileContents, contentType);
        }

        public Task DeleteFileAsync(string fileName)
        {
            string filePath = Path.Combine(_uploadDirectory, fileName);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }

            return Task.CompletedTask;
        }

        private string GetContentType(string fileExtension)
        {
            switch (fileExtension.ToLower())
            {
                case ".jpg":
                case ".jpeg":
                    return "image/jpeg";
                case ".png":
                    return "image/png";
                case ".gif":
                    return "image/gif";
                case ".pdf":
                    return "application/pdf";
                default:
                    return "application/octet-stream";
            }
        }
    }
}