using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wpf_playground.Model
{
    public class TestResult
    {
        public UserInfo UserInfo { get; set; }
        public IList<ClickHistory> ClickHistoryList { get; set; }
        
    }
}
