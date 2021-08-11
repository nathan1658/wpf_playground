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
        public string Gender { get; set; }
        public string Group { get; set; }
        public string DominantHand { get; set; }

        public override string ToString()
        {
            string result = $"Name : {Name}" + "\n";
            result += $"SID: {SID}" + "\n";
            result += $"Age: {Age}" + "\n";
            result += $"Gender: {Gender}" + "\n";
            result += $"Group: {Group}" + "\n";
            result += $"Dominant Hand : {DominantHand}" + "\n";
            return result;
        }
    }
}
