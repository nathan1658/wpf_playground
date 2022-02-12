using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class Config
    {
        public SpeakerConfig TopAuditorySpeaker { get; set; }
        public SpeakerConfig PQAuditorySpeaker { get; set; }
        public SpeakerConfig BottomAuditorySpeaker { get; set; }

        public SpeakerConfig TopTactileSpeaker { get; set; }
        public SpeakerConfig PQTactileSpeaker { get; set; }
        public SpeakerConfig BottomTactileSpeaker { get; set; }

        public string COMPortValue { get; set; }
    }

    public class SpeakerConfig
    {
        public string SpeakerGuid { get; set; }
        public string Hz { get; set; }
    }
}
