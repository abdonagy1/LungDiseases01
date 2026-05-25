using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;

namespace LungDisease.Service.Services
{
    public class AIClient : IAIClient
    {
        private readonly HttpClient _httpClient;

        public AIClient(HttpClient httpClient) => _httpClient = httpClient;

        public async Task<AudioAnalysisResultDto> AnalysisAudioAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Audio file not found", filePath);

            using var form = new MultipartFormDataContent();

            
            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));
            fileContent.Headers.ContentType =
                System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");

            form.Add(fileContent, "file", Path.GetFileName(filePath));

            var Url = "https://coolish-nonarbitrarily-rochell.ngrok-free.dev/predict";
            

            var response = await _httpClient.PostAsync(Url, form);

            response.EnsureSuccessStatusCode();

            var json = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AudioAnalysisResultDto>(json);

            if (result == null)
                throw new Exception("Failed to deserialize AI response");

            return result;
        }
    }
}
