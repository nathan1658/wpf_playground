using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class TestMapping
    {
        public SOAEnum SOA;

        public bool VisualSignal;
        public bool AuditorySignal;
        public bool TactileSignal;

        public bool VisualPQ;
        public bool AuditoryPQ;
        public bool TactilePQ;

        public bool PracticeDone { get; set; }
        public bool TestDone { get; set; }

        public TestMapping(SOAEnum soa, int signal, int pq)
        {
            SOA = soa;

            //VisualSignal = (signal & 1) > 0;
            //AuditorySignal = (signal & 2) > 0;
            //TactileSignal = (signal & 4) > 0;

            //VisualPQ = (pq & 1) > 0;
            //AuditoryPQ = (pq & 2) > 0;
            //TactilePQ = (pq & 4) > 0;

            VisualSignal = signal == 1;
            AuditorySignal = signal == 2;
            TactileSignal = signal == 3;

            VisualPQ = pq == 1;
            AuditoryPQ = pq == 2;
            TactilePQ = pq == 3;
        }
        public override string ToString()
        {
            return $"{VisualSignal}:{AuditorySignal}:{TactileSignal}";
        }
    }

}
