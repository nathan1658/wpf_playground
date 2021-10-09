using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Resources;

namespace wpf_playground
{
    public class AudioHelper
    {

        private AudioHelper()
        {
        }



        public void play(Guid speakerGuid, int duration, bool isLeft)
        {
            Task.Run(async () =>
            {
                var sineWaveProvider = new SineWaveProvider32();
                sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
                sineWaveProvider.Frequency = State.UserInfo.PQHz;
                sineWaveProvider.Amplitude = 0.55f;
                var sampleProvider = sineWaveProvider.ToSampleProvider();

                var stereo = new MonoToStereoSampleProvider(sampleProvider);
                stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
                stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel



                var outputDevice = new DirectSoundOut(speakerGuid);
                outputDevice.Init(stereo);
                outputDevice.Play();
                await Task.Delay(duration);
                outputDevice.Stop();
                outputDevice.Dispose();
                outputDevice = null;
            });
        }

        public static readonly AudioHelper Instance = new AudioHelper();
    }
}
