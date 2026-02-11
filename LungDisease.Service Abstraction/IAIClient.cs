using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;

namespace LungDisease.Service_Abstraction
{
    public interface IAIClient
    {
        Task<AudioAnalysisResultDto> AnalysisAudioAsync(string filePath);
    }
}
