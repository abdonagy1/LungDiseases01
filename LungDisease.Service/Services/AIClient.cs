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

        public async Task<Result<AudioAnalysisResultDto>> AnalysisAudioAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return Error.NotFound("Audio file not found");

            using var form = new MultipartFormDataContent();

            var fileContent = new ByteArrayContent(await File.ReadAllBytesAsync(filePath));

            fileContent.Headers.ContentType =
                System.Net.Http.Headers.MediaTypeHeaderValue.Parse("audio/wav");

            form.Add(fileContent, "audioFile", Path.GetFileName(filePath));

            var url = "https://albraaalqady-lung-disease-api.hf.space/predict";

            var response = await _httpClient.PostAsync(url, form);

            var responseBody = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
                return Error.Failure(
                    $"AI API Error: {(int)response.StatusCode} - {responseBody}");

            var result = JsonSerializer.Deserialize<AudioAnalysisResultDto>(
                responseBody,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (result == null)
                return Error.Failure("Failed to deserialize AI response");

            return result;
        }
    }
}
