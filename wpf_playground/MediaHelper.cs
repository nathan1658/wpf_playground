using AudioPlayerApp;
using SharpDX.IO;
using SharpDX.MediaFoundation;
using SharpDX.XAudio2;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class MediaHelper
    {
        public static bool initalized = false;
        private static AudioPlayer audioPlayer;
        private static object lockAudio = new object();

        public MediaHelper()
        {
            if (!initalized)
            {
                init();
            }
        }

        public void Play()
        {
            lock (lockAudio)
            {
                if (audioPlayer != null)
                {

                    if (audioPlayer.State == AudioPlayerState.Playing)
                    {

                        audioPlayer.Position = TimeSpan.Zero;                    

                    }
                    else
                    {
                        audioPlayer.Play();
                    }
                }
            }
        }

        void init()
        {
            // This is mandatory when using any of SharpDX.MediaFoundation classes
            MediaManager.Startup();
            // Starts The XAudio2 engine
            var xaudio2 = new XAudio2();
            xaudio2.StartEngine();
            MasteringVoice masteringVoice = new MasteringVoice(xaudio2);
            Stream fs = new NativeFileStream(@"C:\Users\Nathan\Documents\vb_cityu\Resources\Windows XP Error.wav", NativeFileMode.Open, NativeFileAccess.Read);
            audioPlayer = new AudioPlayer(xaudio2, fs);
            initalized = true;
        }
    }
}
