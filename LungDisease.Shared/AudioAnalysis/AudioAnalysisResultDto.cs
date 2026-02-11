using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LungDisease.Shared.AudioAnalysis
{
    public class AudioAnalysisResultDto
    {
        public string Disease { get; set; } = default!;
        public double Confidence { get; set; } = default!;
        public string Recommendation { get; set; } = default!;
    }
}
