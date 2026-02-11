using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;
using Microsoft.AspNetCore.Http;

namespace LungDisease.Service.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IAIClient _aiClient;
        private readonly string _uploadsDir = "Uploads";

        public AttachmentService(IAIClient aiClient)
        {
            _aiClient = aiClient;
            
            Directory.CreateDirectory(_uploadsDir);
        }
        public async Task<Result<AudioAnalysisResultDto>> AnalysisAsync(IFormFile audioFile)
        {
            if (audioFile == null || audioFile.Length == 0)
                throw new ArgumentException("Audio file is required");

            var filePath = Path.Combine(_uploadsDir, audioFile.FileName);

            
            await using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await audioFile.CopyToAsync(stream);
            }

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
    }
}
