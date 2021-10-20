using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class AuditoryPQ : MyBaseUserControl
    {
        public bool IsLeft { get; set; }
        private SineWaveProvider32 sineWaveProvider;
        private DirectSoundOut outputDevice;

        public AuditoryPQ(bool isLeft, int hz, Guid deviceGuid)
        {
            this.IsLeft = isLeft;
            sineWaveProvider = new SineWaveProvider32();
            sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
            sineWaveProvider.Frequency = hz;
            sineWaveProvider.Amplitude = 0.55f;
            var sampleProvider = sineWaveProvider.ToSampleProvider();

            var stereo = new MonoToStereoSampleProvider(sampleProvider);
            stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
            stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel



            outputDevice = new DirectSoundOut(deviceGuid);
            outputDevice.Init(stereo);

        }

        ~AuditoryPQ()
        {
            outputDevice.Dispose();
            outputDevice = null;
        }

        public override bool Click()
        {
            // not used
            return true;
        }

        public override void Disable()
        {
            outputDevice.Stop();

        }

        public override void Enable()
        {
            outputDevice.Play();

        }
    }
}
