using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace LungDisease.Service.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAIClient _aiClient;
        private readonly IWebHostEnvironment _webHost;

        private readonly string _uploadsDir;
        private readonly string[] allowedExtensions = { ".mp3", ".wav", ".ogg", ".m4a" };
        private readonly long MaxFileSize = 5 * 1024 * 1024;

        public AttachmentService(IAIClient aiClient, IWebHostEnvironment webHost)
        {
            _aiClient = aiClient;
            _webHost = webHost;

            _uploadsDir = Path.Combine(_webHost.ContentRootPath, "Uploads");
            Directory.CreateDirectory(_uploadsDir);
        }

        public async Task<Result<AudioAnalysisResultDto>> AnalysisAsync(IFormFile audioFile)
        {
            if (audioFile is null || audioFile.Length == 0)
                throw new ArgumentException("Audio file is required");

            if (audioFile.Length > MaxFileSize)
                return Error.Failure("File too large");

            var extension = Path.GetExtension(audioFile.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return Error.Failure("Invalid file type");

            var fileName = $"{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(_uploadsDir, fileName);

            // 1. Save file (IMPORTANT: close stream immediately)
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

            try
            {
                // 2. Send to AI (file is NOT locked anymore)
                var aiResult = await _aiClient.AnalysisAudioAsync(filePath);

                return aiResult;
            }
            finally
            {
                // 3. Safe delete
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        #region Upload Image (fixed naming bug)

        public async Task<Result<string?>> UploadAsync(string folderName, IFormFile file)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(folderName) || file is null || file.Length == 0)
                    return Error.Failure("Invalid input");

                if (file.Length > MaxFileSize)
                    return Error.Failure("File too large");

                var extension = Path.GetExtension(file.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return Error.Failure("Invalid file type");

                var folderPath = Path.Combine(_webHost.WebRootPath, "images", folderName);
                Directory.CreateDirectory(folderPath);

                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(folderPath, fileName);

                await using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return Error.Failure("Upload failed");
            }
        }


        public Result<bool> Delete(string fileName, string folderName)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(fileName) || string.IsNullOrWhiteSpace(folderName))
                    return Error.Failure("Invalid input");

                var path = Path.Combine(_webHost.WebRootPath, "images", folderName, fileName);

                if (!File.Exists(path))
                    return Error.NotFound();

                File.Delete(path);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        #endregion
    }
}
