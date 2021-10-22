using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground
{
    public class TactilePQ : AuditoryPQ
    {
        public TactilePQ(bool isLeft, int hz, Guid deviceGuid) : base(isLeft, hz, deviceGuid)
        {
        }
    }
}
