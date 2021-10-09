using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class AuditoryTarget : MyBaseUserControl
    {
        public bool IsLeft { get; set; }
        private SineWaveProvider32 sineWaveProvider;
        private DirectSoundOut outputDevice;

        public AuditoryTarget(DirectSoundDeviceInfo deviceInfo,  bool isLeft)
        {
            this.IsLeft = isLeft;
            sineWaveProvider = new SineWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            sineWaveProvider.Frequency = State.UserInfo.PQHz;
            sineWaveProvider.Amplitude = 0.55f;
            var sampleProvider = sineWaveProvider.ToSampleProvider();

            var stereo = new MonoToStereoSampleProvider(sampleProvider);
            stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
            stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel



            outputDevice = new DirectSoundOut(deviceInfo.Guid);
            outputDevice.Init(stereo);

        }

        public override bool Triggered
        {
            get
            {
                return _triggered;
            }
            set
            {
                _triggered = value;
                if(_triggered)
                {
                    outputDevice.Play();
                }else
                {
                    outputDevice.Stop();
                }
            }
        }

        ~AuditoryTarget()
        {
            outputDevice.Dispose();
            outputDevice = null;
        }

        public override bool Click()
        {
            if (this.Triggered)
            {
                this.Triggered = false;
                return true;
            }
            return false;
        }

        public override void Disable()
        {
            outputDevice.Stop();
        }

        public override void Enable()
        {
            if (this.Triggered) return;
            this.Triggered = true;
        }
    }
}
