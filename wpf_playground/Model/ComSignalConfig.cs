using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class ComSignalConfig : IEquatable<ComSignalConfig>
    {
        public SOAEnum Soa;
        public SignalModeEnum SignalMode;
        public PQModeEnum PQMode;

        public ComSignalConfig(SignalModeEnum signalMode, PQModeEnum pqMode, SOAEnum soa)
        {
            this.Soa = soa;
            this.SignalMode = signalMode;
            this.PQMode = pqMode;
        }
 
        public bool Equals(ComSignalConfig other)
        {
            return other != null &&
                   Soa == other.Soa &&
                   SignalMode == other.SignalMode &&
                   PQMode == other.PQMode;
        }

        public override int GetHashCode()
        {
            var hashCode = -511358471;
            hashCode = hashCode * -1521134295 + Soa.GetHashCode();
            hashCode = hashCode * -1521134295 + SignalMode.GetHashCode();
            hashCode = hashCode * -1521134295 + PQMode.GetHashCode();
            return hashCode;
        }
    }
}
