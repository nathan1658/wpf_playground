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
        public GenderEnum Gender { get; set; }
        public GroupEnum Group { get; set; }
        public DominantHandEnum DominantHand { get; set; }
        public LevelEnum Level { get; set; }
        public SignalModeEnum SignalMode { get; set; }
        public PQModeEnum PQMode { get; set; }
        public SOAEnum SOA { get; set; }
        public MappingEnum Mapping { get; set; }

        public override string ToString()
        {
            string result = $"Name : {Name}" + "\n";
            result += $"SID: {SID}" + "\n";
            result += $"Age: {Age}" + "\n";
            result += $"Gender: {Gender}" + "\n";
            result += $"Group: {Group}" + "\n";
            result += $"Dominant Hand : {DominantHand}" + "\n";
            result += $"Level : {Level}" + "\n";
            result += $"Signal Mode : {SignalMode}" + "\n";
            result += $"PQ Mode : {PQMode}" + "\n";
            return result;
        }
    }
}
