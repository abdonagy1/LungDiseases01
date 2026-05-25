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
        private readonly string _uploadsDir = "Uploads";
        private readonly string[] allowedExtensions = { ".jpg", ".jpeg", ".png" };
        private readonly long MaxFileSize = 5 * 1024 * 1024;

        public AttachmentService(IAIClient aiClient,IWebHostEnvironment webHost)
        {
            _aiClient = aiClient;
            _webHost = webHost;
            Directory.CreateDirectory(_uploadsDir);
        }
        public async Task<Result<AudioAnalysisResultDto>> AnalysisAsync(IFormFile audioFile)
        {
            if (audioFile is  null || audioFile.Length == 0)
                throw new ArgumentException("Audio file is required");

            var filePath = Path.Combine(_uploadsDir, audioFile.FileName);


            await using var stream = new FileStream(filePath, FileMode.Create);
                await audioFile.CopyToAsync(stream);
            

            try
            {
                
                var aiResult = await _aiClient.AnalysisAudioAsync(filePath);

                return aiResult;
            }
            finally
            {
                
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
        }

        #region Delete and Upload
        public Result<bool> Delete(string FileName, string FolderName)
        {
            try
            {
                if (string.IsNullOrEmpty(FileName) || string.IsNullOrEmpty(FolderName))
                    return Error.Failure("FileName.Failure Or FolderName.Failure", "FileName Is Empty Or FolderName Is Empty");
                var FallPath = Path.Combine(_webHost.WebRootPath, "images", FolderName, FileName);

                if (File.Exists(FallPath))
                {
                    File.Delete(FallPath);
                    return true;
                }
                return Error.NotFound();

            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed To Delete File with Name {FileName} : {ex}");
                return false;
            }
        }

        public async Task<Result<string?>> UploadAsync(string FolderName, IFormFile File)
        {
            try
            {
                if (FolderName is null || File is null || File.Length == 0)
                    return Error.NotFound("FolderName.NotFound Or File.NotFound");

                if (File.Length > MaxFileSize)
                    return null;

                var extension = Path.GetExtension(File.FileName).ToLower();

                if (!allowedExtensions.Contains(extension))
                    return null;

                var FolderPath = Path.Combine(_webHost.WebRootPath, "images", FolderName);

                if (!Directory.Exists(FolderPath))
                    Directory.CreateDirectory(FolderPath);

                var fileName = Guid.NewGuid().ToString() + extension;

                var FilePath = Path.Combine(FolderPath, fileName);

                await using var fileStream = new FileStream(FilePath, FileMode.Create);

                await File.CopyToAsync(fileStream);

                return fileName;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed ToUpload File To Folder = {FolderName} : {ex}");
                return null;
            }
        } 
        #endregion
    }
}
