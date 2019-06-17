using System;
using System.Collections.Generic;
using System.Linq;
using System.Speech.Synthesis;
using System.Text;
using System.Threading.Tasks;

namespace CFLMedCab.Infrastructure.DeviceHelper
{
    class SpeakerHelper
    {
        public static void Sperker(string str)
        {
			SpeechSynthesizer synth = new SpeechSynthesizer
            {
                Rate = 3,
                Volume = 100
            };

            //配置和声音输出  
            synth.SetOutputToDefaultAudioDevice();

            synth.Speak(str);
            synth.Dispose();
        }
    }
}
