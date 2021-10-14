using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace wpf_playground
{
    public class TactileTarget : AuditoryTarget
    {
        public TactileTarget(DirectSoundDeviceInfo deviceInfo, float frequency, bool isLeft) : base(deviceInfo, frequency, isLeft)
        {
        }
    }
}
