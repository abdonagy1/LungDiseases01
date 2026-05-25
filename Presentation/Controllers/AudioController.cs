using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LungDisease.Service_Abstraction;
using LungDisease.Shared.AudioAnalysis;
using LungDisease.Shared.Common_Result;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Presentation.Controllers
{
    [Authorize]
    public class AudioController : ApiBaseController
    {

        private readonly IAttachmentService _analysisService;

        public AudioController(IAttachmentService analysisService)
        {

            _analysisService = analysisService;
        }

        

        [HttpPost("analysis")]
        public async Task<ActionResult<AudioAnalysisResultDto>> AnalysisAudioAsync(IFormFile audioFile)
        {

            var result = await _analysisService.AnalysisAsync(audioFile);

            
            return HandleResult(result);


        }
    }
}
