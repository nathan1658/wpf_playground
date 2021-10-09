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
        private DirectSoundDeviceInfo deviceInfo;
        private float _frequency;
        private bool initialized = false;
        public float Frequency
        {
            get
            {
                return _frequency;
            }
            set
            {
                _frequency = value;
                if (initialized)
                    setOutputDevice();

            }
        }

        void setOutputDevice()
        {
            sineWaveProvider = new SineWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            sineWaveProvider.Frequency = _frequency;
            sineWaveProvider.Amplitude = 0.55f;
            var sampleProvider = sineWaveProvider.ToSampleProvider();

            var stereo = new MonoToStereoSampleProvider(sampleProvider);
            stereo.LeftVolume = IsLeft ? 1.0f : 0f; // silence in left channel
            stereo.RightVolume = IsLeft ? 0f : 1.0f; // full volume in right channel



            outputDevice = new DirectSoundOut(deviceInfo.Guid);
            outputDevice.Init(stereo);
        }

        public AuditoryTarget(DirectSoundDeviceInfo deviceInfo, float frequency, bool isLeft)
        {
            this.IsLeft = isLeft;
            this.deviceInfo = deviceInfo;
            this.Frequency = frequency;
            setOutputDevice();
            initialized = true;
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
                if (_triggered)
                {
                    outputDevice.Play();
                }
                else
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
            this.Triggered = false;
        }

        public override void Enable()
        {
            if (this.Triggered) return;
            this.Triggered = true;
        }
    }
}
