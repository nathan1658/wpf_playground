using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class UserInfo
    {
        public string Name { get; set; }
        public string SID { get; set; }
        public string Age { get; set; }
        public GenderEnum Gender { get; set; } = GenderEnum.Male;
        public DominantHandEnum DominantHand { get; set; } = DominantHandEnum.Right;
        public LevelEnum Level { get; set; } = LevelEnum.L50;

        public bool SignalVisualChecked { get; set; }
        public bool SignalAuditoryChecked { get; set; }
        public bool SignalTactileChecked { get; set; }

        public bool PQVisualChecked { get; set; }
        public bool PQAuditoryChecked { get; set; }
        public bool PQTactileChecked { get; set; }

        public SOAEnum SOA { get; set; } = SOAEnum.Soa200;
        public int PQHz { get; set; } = 1000;
        public int TopSpeakerHz { get; set; } = 1000;
        public int BottomSpeakerHz { get; set; } = 1000;

        public int TactilePQHz { get; set; } = 100;
        public int TactileTopSpeakerHz { get; set; } = 100;
        public int TactileBottomSpeakerHz { get; set; } = 100;
        public override string ToString()
        {
            string result = $"Name : {Name}" + "\n";
            result += $"SID: {SID}" + "\n";
            result += $"Age: {Age}" + "\n";
            result += $"Gender: {Gender}" + "\n";
            result += $"Dominant Hand : {DominantHand}" + "\n";
            result += $"Level : {Level}" + "\n";
            result += $"SOA : {SOA}" + "\n";
            result += $"PQ Hz : {PQHz}" + "\n";
            result += $"Top Speaker Hz : {TopSpeakerHz}" + "\n";
            result += $"Bottom Speaker Hz : {BottomSpeakerHz}" + "\n";
            result += $"Tactile PQ Hz : {TactilePQHz}" + "\n";
            result += $"Tactile Top Speaker Hz : {TactileTopSpeakerHz}" + "\n";
            result += $"Tactile Bottom Speaker Hz : {TactileBottomSpeakerHz}" + "\n";
            return result;
        }
    }
}
