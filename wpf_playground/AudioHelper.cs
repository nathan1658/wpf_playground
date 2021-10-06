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
        private AudioFileReader player;

        private AudioHelper()
        {
            var currentDir = Path.GetDirectoryName(System.Diagnostics.Process.GetCurrentProcess().MainModule.FileName);
            String filePath = Path.Combine(currentDir, "Resources/error.wav");
            player = new AudioFileReader(filePath);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="duration">Specify the sound duration in ms</param>
        /// <param name="isLeft">play the sound in left</param>
        //public void play(int duration, bool isLeft)
        //{            
        //    _task = Task.Run(() =>
        //    {
        //        player.Position = 0;
        //        using (var output = new WaveOutEvent())
        //        {
        //            // convert our mono ISampleProvider to stereo
        //            var stereo = new MonoToStereoSampleProvider(player);
        //            stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
        //            stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel


        //            var semitone = Math.Pow(2, 1.0 / 12);
        //            var upOneTone = semitone * semitone;
        //            var downOneTone = 1.0 / upOneTone;

        //            var pitch = new SmbPitchShiftingSampleProvider(stereo);
        //            pitch.PitchFactor = (float)downOneTone;



        //            //output.Init(pitch);
        //            output.Init(stereo);
        //            output.Play();
        //            Stopwatch timer = new Stopwatch();
        //            timer.Start();
        //            while (output.PlaybackState == PlaybackState.Playing && timer.ElapsedMilliseconds < duration)
        //            {
        //                Thread.Sleep(10);
        //            }
        //            output.Stop();
        //        }
        //    });
        //}


        public void play(int duration, bool isLeft)
        {
            Task.Run(async () =>
            {
                WaveOut waveOut;

                var sineWaveProvider = new SineWaveProvider32();
                sineWaveProvider.SetWaveFormat(16000, 1); // 16kHz mono
                sineWaveProvider.Frequency = State.UserInfo.Hz;
                sineWaveProvider.Amplitude = 0.55f;
                var sampleProvider = sineWaveProvider.ToSampleProvider();

                var stereo = new MonoToStereoSampleProvider(sampleProvider);
                stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
                stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel

                waveOut = new WaveOut();
                waveOut.Init(stereo);

                waveOut.Play();
                await Task.Delay(duration);
                waveOut.Stop();
                waveOut.Dispose();
                waveOut = null;
            });
        }

        public static readonly AudioHelper Instance = new AudioHelper();
    }
}
