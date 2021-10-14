using NAudio.Wave;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using wpf_playground.Model;

namespace wpf_playground
{
    public class State
    {
        public static UserInfo UserInfo = new UserInfo();
        public static bool DebugMode = false;
        //Default is 6
        public static int ClickCountForEachButton = 6;


        public static Key TopLeftKey;
        public static Key TopRightKey;
        public static Key BottomLeftKey;
        public static Key BottomRightKey;

        public static DirectSoundDeviceInfo PQSpeaker;
        public static DirectSoundDeviceInfo TopSpeaker;
        public static DirectSoundDeviceInfo BottomSpeaker;

        public static DirectSoundDeviceInfo TactilePQSpeaker;
        public static DirectSoundDeviceInfo TactileTopSpeaker;
        public static DirectSoundDeviceInfo TactileBottomSpeaker;
    }
}
