using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class TactileSignal: AuditorySignal
    {
        public TactileSignal(DirectSoundDeviceInfo deviceInfo, float frequency, bool isLeft):base(deviceInfo, frequency, isLeft)
        {
        }
    }
}
