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
        private Task _task;
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
        public void play(int duration, bool isLeft)
        {
            if (_task != null)
                _task = null;
            _task = Task.Run(() =>
            {
                player.Position = 0;
                using (var output = new WaveOutEvent())                
                {
                    // convert our mono ISampleProvider to stereo
                    var stereo = new MonoToStereoSampleProvider(player);
                    stereo.LeftVolume = isLeft ? 1.0f : 0f; // silence in left channel
                    stereo.RightVolume = isLeft ? 0f : 1.0f; // full volume in right channel

                    output.Init(stereo);
                    output.Play();
                    Stopwatch timer = new Stopwatch();
                    timer.Start();
                    while (output.PlaybackState == PlaybackState.Playing && timer.ElapsedMilliseconds < duration)
                    {
                        Thread.Sleep(100);
                    }
                }
            });
        }

        public static readonly AudioHelper Instance = new AudioHelper();
    }
}
