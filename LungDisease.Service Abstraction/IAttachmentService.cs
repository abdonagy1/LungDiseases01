using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;
using Microsoft.AspNetCore.Http;

namespace LungDisease.Service_Abstraction
{
    public interface IAttachmentService
    {
        Task<Result<AudioAnalysisResultDto>> AnalysisAsync(IFormFile audioFile);

        Task<Result<string?>> UploadAsync(string FolderName, IFormFile File);
        Result<bool> Delete(string FileName,string FolderName);
    }
}
