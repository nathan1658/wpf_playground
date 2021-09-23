using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class ClickHistory
    {

        public int Id { get; set; }
        public DateTime ClickDate { get; set; }
        public double ElapsedTime { get; set; }
        public double ReactionTime { get; set; }
        public double Distance { get; set; }
        public ClickState ClickState{ get; set; }
        public int Delay { get; set; }
    }
}
